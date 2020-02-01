using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Xms.Api.Core.Swagger;
using Xms.Core;

namespace Xms.Api.Identity
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
            services.AddControllersWithViews();
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(new List<IdentityResource> {
                    new IdentityResources.OpenId()
                    , new IdentityResources.Profile()
                })
                .AddInMemoryApiResources(new List<ApiResource> {
                    new ApiResource("Xms.Api", "Xms.Api") //api scopes,这里要跟AllowedScopes一致
                })
                .AddInMemoryClients(new[] {new Client {
                    ClientId = "Xms.Web"
                    , AllowedGrantTypes = GrantTypes.ResourceOwnerPassword
                    , ClientSecrets = {
                        new Secret("pwdsecret".Sha256())
                    }
                    , AllowedScopes = { "Xms.Api" }
                }})
                //.AddTestUsers(new List<IdentityServer4.Test.TestUser> {
                //    new IdentityServer4.Test.TestUser
                //    {
                //        SubjectId = "1"
                //        , Username = "admin"
                //        , Password = "888888"
                //    }
                //});
                .AddCustomUserStore();
            services.RegisterAll(Configuration);
            //swagger
            services.AddXmsSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();

            app.UseIdentityServer();
            //swagger ui
            app.UseXmsSwaggerUI();
            //app.UseMvc();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
