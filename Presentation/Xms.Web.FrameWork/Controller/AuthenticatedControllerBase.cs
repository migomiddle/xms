using Microsoft.AspNetCore.Mvc;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Filters;

namespace Xms.Web.Framework.Controller
{
    /// <summary>
    /// 登录状态验证控制器基类
    /// </summary>
    [TypeFilter(typeof(InitializationFilterAttribute), Order = 0)]
    [TypeFilter(typeof(IdentityFilterAttribute), Order = 1)]
    public class AuthenticatedControllerBase : WebControllerBase
    {
        protected AuthenticatedControllerBase(IWebAppContext appContext) : base(appContext)
        {
        }
    }
}