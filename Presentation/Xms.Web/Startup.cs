using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xms.Module;
using Xms.Web.Framework.Infrastructure;

namespace Xms.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //支持跨域
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        builder => builder.AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowCredentials());
            //});
            services.AddWebDefaults(Configuration);
            services.AddSession();
            services.RegisterModules();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)//, IModuleRegistrar moduleRegistrar)
        {
            loggerFactory.AddLog4Net("log4net.config", true);
            //moduleRegistrar.RegisterAll();
            //if (env.IsDevelopment())

            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Error/index");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    //app.UseHsts();
            //}
            //app.UseHttpsRedirection();
            app.UseXmsStaticFiles(env, Configuration);
            //app.UseCookiePolicy();
            app.UseSession();
            //app.UseAuthentication();
            //app.UseExceptionHandlerMiddleWare();
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(name: "org",
            //                    template: "{org}/{controller=Home}/{action=Index}");
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //    routes.MapRoute(
            //        "home",
            //        "{org}/index",
            //                      new { controller = "home", action = "index" });
            //    routes.MapRoute("error",
            //                    "error/{action}",
            //                    new { controller = "error", action = "index" });
            //    routes.MapRoute(
            //     name: "customize",
            //     template: "{org}/{area:exists}/{controller=Home}/{action=Index}/{id?}"
            //   );
            //    routes.MapRoute(
            //        "customize_home",
            //        "{org}/customize",
            //                      new { area = "customize", controller = "home", action = "index" });
            //    routes.MapRoute(
            //        "customize_home_index",
            //        "{org}/customize/index",
            //                      new { area = "customize", controller = "home", action = "index" });
            //});
            app.UseWebDefaults();
        }
    }
}