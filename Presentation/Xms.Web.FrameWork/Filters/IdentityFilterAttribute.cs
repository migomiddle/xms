using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xms.Infrastructure.Utility;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;

namespace Xms.Web.Framework.Filters
{
    /// <summary>
    /// 登录验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class IdentityFilterAttribute : ActionFilterAttribute
    {
        private readonly IWebAppContext _appContext;

        public IdentityFilterAttribute(IWebAppContext appContext)
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

            //验证登录
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var loginUrl = _appContext.LoginUrl;
                //loginUrl = _appContext.OrganizationUniqueName.HasValue() ? "/" + _appContext.OrganizationUniqueName + loginUrl : loginUrl;
                if (filterContext.HttpContext.IsRequestJson())
                {
                    filterContext.Result = new JsonResult(new JsonResultObject() { IsSuccess = false, StatusName = "Signin", Content = "请先登录", Url = loginUrl });
                }
                else
                {
                    var thisPageUrl = filterContext.HttpContext.GetThisPageUrl(includeQueryString: true);
                    filterContext.Result = new RedirectResult(loginUrl + (thisPageUrl.IsCaseInsensitiveEqual(loginUrl) ? "" : "?returnurl=" + thisPageUrl));
                }
                return;
            }
            else
            {
                var OrgUniqueName = filterContext.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "OrgUniqueName");
                if (OrgUniqueName != null)
                {
                    //根目录跳转
                    if (!_appContext.OrganizationUniqueName.HasValue())
                    {
                        var thisPageUrl = filterContext.HttpContext.GetThisPageUrl(includeQueryString: true);
                        //var url = _appContext.OrganizationUniqueName.HasValue() ? "/" + _appContext.OrganizationUniqueName + thisPageUrl : thisPageUrl;
                        var url = "/" + OrgUniqueName.Value + thisPageUrl;
                        filterContext.Result = new RedirectResult(url);
                    }
                    else
                    {
                        //禁止不退出数据库切换
                        if (OrgUniqueName.Value != _appContext.OrganizationUniqueName)
                        {
                            var url = filterContext.HttpContext.GetThisPageUrl(includeQueryString: true);
                            Regex r = new Regex("/" + _appContext.OrganizationUniqueName);
                            url = r.Replace(url, "/" + OrgUniqueName.Value, 1);
                            filterContext.Result = new RedirectResult(url);
                        }
                    }
                }
                return;
            }
        }
    }
}