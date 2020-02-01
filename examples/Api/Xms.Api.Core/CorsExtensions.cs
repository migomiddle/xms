using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Xms.Api.Core
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddXmsCors(this IServiceCollection services)
        {
            //支持跨域
            services.AddCors(options =>
            {
            options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
                    //.AllowCredentials());
                    
            });
            return services;
        }

        public static void UseXmsCors(this IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");
            //app.UseCors();
        }
    }
}
