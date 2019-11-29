using Microsoft.AspNetCore.Mvc;

using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Filters;

namespace Xms.Web.Controllers
{
    [TypeFilter(typeof(InitializationFilterAttribute), Order = 0)]
    [TypeFilter(typeof(IdentityFilterAttribute), Order = 1)]
    [TypeFilter(typeof(OrganizationFilterAttribute), Order = 2)]
    public class HomeController : XmsControllerBase
    {
        public HomeController(IWebAppContext appContext)
            : base(appContext)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}