using Microsoft.AspNetCore.Mvc;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Filters;

namespace Xms.Web.Framework.Controller
{
    /// <summary>
    /// 授权验证控制器基类
    /// </summary>
    [TypeFilter(typeof(InitializationFilterAttribute), Order = 0)]
    [TypeFilter(typeof(IdentityFilterAttribute), Order = 1)]
    [TypeFilter(typeof(AuthorizeFilterAttribute), Order = 2)]
    public class AuthorizedControllerBase : WebControllerBase
    {
        protected AuthorizedControllerBase(IWebAppContext appContext) : base(appContext)
        {
        }
    }
}