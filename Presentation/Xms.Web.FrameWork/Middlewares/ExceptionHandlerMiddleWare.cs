using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xms.Core;

namespace Xms.Web.Framework.Middlewares
{
    /// <summary>
    /// 异常处理中间件
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        //private readonly IExceptionHandlerFactory _exceptionHandlerFactory;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                try
                {
                    await HandleExceptionAsync(context, ex).ConfigureAwait(false);
                }
                catch { }
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (exception == null)
            {
                return;
            }

            await WriteExceptionAsync(context, exception).ConfigureAwait(false);
        }

        private async Task WriteExceptionAsync(HttpContext context, Exception exception)
        {
            //记录日志
            _logger?.LogError(exception, "");

            //状态码
            var response = context.Response;
            //response.ContentType = context.Request.Headers["Accept"];
            //custom exception handler
            var exceptionHandler = ((IExceptionHandlerFactory)context.RequestServices.GetService(typeof(IExceptionHandlerFactory))).Get(exception);
            exceptionHandler.Handle(context, exception);
            await _next(context);
            //if (exception is UnauthorizedAccessException)
            //{
            //    response.StatusCode = (int)HttpStatusCode.Unauthorized;
            //}
            //else if (exception is Exception)
            //{
            //    response.StatusCode = (int)HttpStatusCode.BadRequest;
            //}
            //else if (exception is XmsException)
            //{
            //    response.StatusCode = (exception as XmsException).StatusCode;
            //}

            //if (context.IsAjaxRequest())
            //{
            //    response.ContentType = "application/json";
            //    await response.WriteAsync(JsonResultObject.Failure(exception.GetBaseException().Message, response.StatusCode).SerializeToJson());
            //}
            //else
            //{
            //    context.Features.Set(exception);
            //    context.Request.Path = new PathString("/error/index");
            //    await _next(context);
            //}
        }
    }

    public static class ExceptionHandlerMiddleWareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleWare(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}