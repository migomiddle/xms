using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Organization;
using Xms.Organization.Domain;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 登录
    /// </summary>
    [AllowAnonymous]
    public class AccountController : XmsControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IOrganizationBaseService _organizationBaseService;

        public AccountController(IWebAppContext appContext
            , IAuthenticationService authenticationService
             , IOrganizationBaseService organizationBaseService
            )
            : base(appContext)
        {
            _authenticationService = authenticationService;
            _organizationBaseService = organizationBaseService;
        }

        /// <summary>
        /// 注销
        /// </summary>
        public IActionResult SignOut()
        {
            _authenticationService.SignOut();
            return Redirect("/");
        }

        /// <summary>
        /// 登录
        /// </summary>
        public IActionResult SignIn()
        {
            if (WebContext.IsSignIn)
            {
                if (WebContext.UrlReferrer.IsNotEmpty() && !WebContext.UrlReferrer.IsCaseInsensitiveEqual(WebContext.Url))
                {
                    return Redirect(WebContext.UrlReferrer);
                }
                return Redirect("~/" + WebContext.OrganizationUniqueName + "/home/index");
            }
            else
            {
                List<OrganizationBase> orglist = _organizationBaseService.Query(n => n.Where(x => x.State == 1));
                SignInModel model = new SignInModel
                {
                    ReturnUrl = HttpContext.GetRouteOrQueryString("returnurl"),
                    OrgUniqueName = WebContext.OrganizationUniqueName,
                    OrgName = WebContext.OrganizationName
                };
                if (model.ReturnUrl.IsNotEmpty() && (model.ReturnUrl.ToLower().IndexOf("/signout") >= 0 || model.ReturnUrl.ToLower().IndexOf("/signin") >= 0))
                {
                    model.ReturnUrl = string.Empty;
                }
                ViewData["orglist"] = orglist;
                return View(model);
            }
        }
    }
}