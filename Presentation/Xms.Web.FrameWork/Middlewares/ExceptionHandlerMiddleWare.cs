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

            //custom exception handler
            var exceptionHandler = ((IExceptionHandlerFactory)context.RequestServices.GetService(typeof(IExceptionHandlerFactory))).Get(exception);
            exceptionHandler.Handle(context, exception);
            context.SetEndpoint(null);
            await _next(context);
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