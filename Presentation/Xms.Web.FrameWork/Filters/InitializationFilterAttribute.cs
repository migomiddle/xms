using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using Xms.Infrastructure.Utility;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;

namespace Xms.Web.Framework.Filters
{
    /// <summary>
    /// 初始化验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class InitializationFilterAttribute : ActionFilterAttribute
    {
        private readonly IWebAppContext _appContext;

        public InitializationFilterAttribute(IWebAppContext appContext)
        {
            _appContext = appContext;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var actionDescriptor = filterContext.ActionDescriptor as ControllerActionDescriptor;
            var isSkip = actionDescriptor.ControllerTypeInfo.GetCustomAttributes(inherit: true)
                  .Any(a => a.GetType().Equals(typeof(AllowAnonymousAttribute)))
                  || actionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                  .Any(a => a.GetType().Equals(typeof(AllowAnonymousAttribute)));
            if (isSkip)
            {
                return;
            }
            //初始化验证
            if (!_appContext.IsInitialization)
            {
                var initializationUrl = _appContext.InitializationUrl;

                if (filterContext.HttpContext.IsRequestJson())
                {
                    filterContext.Result = new JsonResult(new JsonResultObject() { IsSuccess = false, StatusName = "Initialization", Content = "未初始化", Url = initializationUrl });
                }
                else
                {
                    var thisPageUrl = filterContext.HttpContext.GetThisPageUrl(includeQueryString: true);
                    filterContext.Result = new RedirectResult(initializationUrl);
                }
                return;
            }
        }
    }
}