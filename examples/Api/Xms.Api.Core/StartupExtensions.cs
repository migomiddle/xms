using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xms.Core;
using Xms.Identity;

namespace Xms.Api.Core
{

    public static class StartupExtensions
    {
        public static void AddApiDefaults(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews();
            services.AddMemoryCache();
            //services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            
            //services.AddSession();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                // You might want to only set the application cookies over a secure connection:
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            //services.AddAuthorization();
            //services.AddAuthentication("Bearer")//.AddJwtBearer()
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        options.Authority = configuration["IdentityServer:Url"];
            //        options.RequireHttpsMetadata = false;
            //        options.ApiName = "Xms.Api";
            //        //options.SupportedTokens = IdentityServer4.AccessTokenValidation.SupportedTokens.Both;
            //    });
            services.AddAuthentication(XmsAuthenticationDefaults.AuthenticationScheme)
            .AddJwtBearer(XmsAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Authority = configuration["IdentityServer:Url"];
                options.RequireHttpsMetadata = false;
                options.Audience = "Xms.Api";
            });
            //注册所有业务服务类
            services.RegisterAll(configuration);
        }

        public static void UseApiDefaults(this IApplicationBuilder app)
        {


            //app.UseStaticFiles();
            app.UseSession();
            //app.UseMvc();
            app.UseRouting();

            //app.UseCors();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
