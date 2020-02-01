using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xms.Infrastructure.Utility;
using Ocelot.JwtAuthorize;
using Microsoft.OpenApi.Models;

namespace Xms.Api.Core.Swagger
{
    public static class SwaggerGenExtensions
    {
        public static IServiceCollection AddXmsSwaggerGen(this IServiceCollection services
            , Action<XmsSwaggerGenOptions> setupAction = null)
        {
            //services.AddApiJwtAuthorize((context) =>
            //{
            //    return true;
            //});

            var options = new XmsSwaggerGenOptions();
            if (setupAction != null)
            {
                setupAction.Invoke(options);
            }
            else
            {
                var xmlFile = $"{Assembly.GetCallingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.XmlFile = xmlPath;
            }
            if (options.Title.IsEmpty())
            {
                options.Title = Assembly.GetCallingAssembly().GetName().Name;
            }
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(options.Title, new OpenApiInfo
                {
                    Version = options.Version,
                    Title = options.Title
                });

                // Set the comments path for the Swagger JSON and UI.
                c.IncludeXmlComments(options.XmlFile);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { In = ParameterLocation.Header, Description = "请输入带有Bearer的Token", Name = "Authorization", Type = SecuritySchemeType.ApiKey});
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    //{
                    //    "Bearer",
                    //     //new List<string>() { "Bearer" }
                    //     Enumerable.Empty<string>()
                    // }
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                            Id = "Bearer", //The name of the previously defined security scheme.
                            Type = ReferenceType.SecurityScheme
                        }
                    },new List<string>()
                }
                });
            });
            return services;
        }
    }

    public class XmsSwaggerGenOptions
    {
        public string Version { get; set; } = "v1";
        public string Title { get; set; }
        public string XmlFile { get; set; }
    }
}
