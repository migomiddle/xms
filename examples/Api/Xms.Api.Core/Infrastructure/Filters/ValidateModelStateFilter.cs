using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Api.Core.Infrastructure.Filters
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            var validationErrors = context.ModelState
                .Keys
                .SelectMany(k => context.ModelState[k].Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();

            context.HttpContext.Response.ContentType = "application/json";
            var result = JResult.Error(JsonResultObject.Failure("", data: validationErrors, statusCode: 400));

            context.Result = result;
        }
    }
}
