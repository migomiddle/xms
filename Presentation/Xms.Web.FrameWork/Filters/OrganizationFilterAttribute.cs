using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Xms.Infrastructure;
using Xms.Organization;
using Xms.Web.Framework.Context;

namespace Xms.Web.Framework.Filters
{
    /// <summary>
    /// 组织状态验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class OrganizationFilterAttribute : ActionFilterAttribute
    {
        private readonly IWebAppContext _appContext;
        private readonly IOrganizationBaseService _organizationBaseService;

        public OrganizationFilterAttribute(IWebAppContext appContext, IOrganizationBaseService organizationBaseService)
        {
            _appContext = appContext;
            _organizationBaseService = organizationBaseService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_appContext.Org != null)
            {
                var baseOrg = _organizationBaseService.FindByUniqueName(_appContext.OrganizationUniqueName);
                if (baseOrg != null)
                {
                    if (baseOrg.State == 0)
                    {
                        throw new XmsException("组织已禁用");
                    }
                }
                else
                {
                    throw new XmsException("组织不存在");
                }
            }
            else
            {
                throw new XmsException("组织名未指定");
            }
        }
    }
}