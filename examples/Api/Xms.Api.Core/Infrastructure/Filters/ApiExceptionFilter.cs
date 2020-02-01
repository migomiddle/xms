using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Api.Core.Infrastructure.Filters
{
    /// <summary>
    /// api异常处理
    /// </summary>
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ApiExceptionFilter(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public override void OnException(ExceptionContext context)
        {
            if (!_hostingEnvironment.IsDevelopment())
            {
                return;
            }
            context.HttpContext.Response.ContentType = "application/json";
            var result = JResult.Error(JsonResultObject.Failure(context.Exception.GetBaseException().Message, 400));
            context.Result = result;
        }
    }
}
