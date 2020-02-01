using Microsoft.AspNetCore.Builder;
using System;
using System.Reflection;
using Xms.Infrastructure.Utility;

namespace Xms.Api.Core.Swagger
{
    public static class SwaggerUIBuilderExtensions
    {
        public static IApplicationBuilder UseXmsSwaggerUI(this IApplicationBuilder app
            , Action<XmsSwaggerUIOptions> setupAction = null)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();



            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            var options = new XmsSwaggerUIOptions();
            if (setupAction != null)
            {
                setupAction.Invoke(options);
            }
            if(options.Title.IsEmpty())
            {
                options.Title = Assembly.GetCallingAssembly().GetName().Name;
            }
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "{documentName}/{options.Version}/swagger.json";
            }).UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/{options.Title}/{options.Version}/swagger.json", options.Title);
            });
            return app;
        }
    }

    public class XmsSwaggerUIOptions
    {
        public string Version { get; set; } = "v1";
        public string Title { get; set; }
    }
}
