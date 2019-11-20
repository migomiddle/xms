using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using Xms.Infrastructure.Utility;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Web.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        public ErrorController()
        {
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            var msg = "";
            var exception = HttpContext.Features.Get<Exception>();
            if (exception != null)
            {
                msg = exception.Message;
            }
            else
            {
                msg = HttpContext.GetRouteValue("error")?.ToString() ?? HttpContext.Items["error"]?.ToString() ?? "error";
            }
            if (HttpContext.IsAjaxRequest())
            {
                return JResult.Error(msg);
            }
            return View("Prompt", new PromptModel(HttpContext.GetUrlReferrer(), msg));
        }
    }
}