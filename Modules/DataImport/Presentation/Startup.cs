using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Web.Framework.Infrastructure;
using Xms.Identity;
using Xms.Organization.Domain;
using Microsoft.Extensions.Logging;

namespace Xms.Module.DataImport.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebDefaults(Configuration);
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net("log4net.config", true);
            app.UseWebDefaults();
            app.UseSession();

            app.Run(async (context) =>
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    var authenticationService = context.RequestServices.GetService<IAuthenticationService>();
                    authenticationService.SignIn(new SystemUser { LoginName = "admin", Password = "888888", OrganizationId = new System.Guid("00000000-0000-0000-00AA-110000000011") });
                }
            });
        }
    }
}
