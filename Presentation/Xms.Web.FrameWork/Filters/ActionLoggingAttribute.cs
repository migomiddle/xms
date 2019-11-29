using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using Xms.Logging.AppLog;
using Xms.Web.Framework.Context;

namespace Xms.Web.Framework.Filters
{
    /// <summary>
    /// 记录访问日志
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ActionLoggingAttribute : ActionFilterAttribute
    {
        private readonly IWebAppContext _appContext;
        public readonly ILogService _logService;

        public ActionLoggingAttribute(IWebAppContext appContext, ILogService logService)
        {
            _appContext = appContext;
            _logService = logService;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!_appContext.PlatformSettings.LogEnabled)
            {
                return;
            }
            var actionDescriptor = filterContext.ActionDescriptor as ControllerActionDescriptor;
            var isSkip = actionDescriptor.ControllerTypeInfo.GetCustomAttributes(inherit: true)
                  .Any(a => a.GetType().Equals(typeof(AllowAnonymousAttribute)))
                  || actionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                  .Any(a => a.GetType().Equals(typeof(AllowAnonymousAttribute)));
            if (isSkip)
            {
                return;
            }
            object[] attrs = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
            string message = string.Empty;
            if (attrs.Length > 0)
            {
                message = (attrs[0] as System.ComponentModel.DescriptionAttribute).Description;
            }
            var controllerName = actionDescriptor.ControllerName;
            var actionName = actionDescriptor.ActionName;
            if (filterContext.Exception == null)
            {
                _logService.Information(message + " " + controllerName + "." + actionName);
            }
            else
            {
                _logService.Error(message + " " + filterContext.Exception.Message, filterContext.Exception);
            }
        }
    }
}