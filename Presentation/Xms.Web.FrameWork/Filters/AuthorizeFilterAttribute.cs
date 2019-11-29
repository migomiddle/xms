using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Security.Principal;
using Xms.Web.Framework.Context;

namespace Xms.Web.Framework.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeFilterAttribute : ActionFilterAttribute
    {
        private readonly IWebAppContext _appContext;
        private readonly IPermissionService _permissionService;

        public AuthorizeFilterAttribute(IWebAppContext appContext, IPermissionService permissionService)
        {
            _appContext = appContext;
            _permissionService = permissionService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_appContext.CurrentUser.IsSuperAdmin)//超级管理员不验证权限
            {
                return;
            }
            //验证权限
            else
            {
                var isViewPage = true;// !filterContext.IsChildAction;

                if (this.Valid(filterContext, isViewPage) == false)
                {
                    throw new XmsUnauthorizedException("没有权限");
                }
            }
        }

        /// <summary>
        /// 菜单权限判断
        /// </summary>
        /// <param name="filterContext"></param>
        /// <param name="isViewPage"></param>
        /// <returns></returns>
        protected virtual bool Valid(ActionExecutingContext filterContext, bool isViewPage)
        {
            if (filterContext.HttpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            var url = filterContext.HttpContext.GetThisPageUrl(false).Replace("/" + _appContext.OrganizationUniqueName, "");

            return _permissionService.HasPermission(url);
        }
    }
}