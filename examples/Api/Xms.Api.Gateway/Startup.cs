using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Kubernetes;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using System.Linq;
using Microsoft.OpenApi.Models;
using System;

namespace Xms.Api.Gateway
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
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //services.AddMvcCore()
            //.AddAuthorization()
            //.AddJsonFormatters();
            services.AddOcelot();
            //.AddConsul()
            //.AddConfigStoredInConsul();
            //.AddKubernetes();

            //网关不开启鉴权，各服务自行验证身份
            //services.AddAuthentication()
            //    .AddIdentityServerAuthentication("XmsAuthKey", o =>
            //    {
            //        o.Authority = Configuration["IdentityServer:Url"];
            //        o.RequireHttpsMetadata = false;
            //        o.ApiName = "Xms.Api";
            //        o.SupportedTokens = IdentityServer4.AccessTokenValidation.SupportedTokens.Both;
            //    });


            //services.AddMvc();
            services.AddControllersWithViews();



            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ApiGateway", new OpenApiInfo { Title = "网关服务", Version = "v1", Contact = new OpenApiContact { Name = "SwaggerOcelot", Url = new Uri("http://10.10.10.10") }, Description = "网关平台" });
            });

            //services.AddSwaggerGen(options =>
            //{
            //    options.SwaggerDoc($"{Configuration["Swagger:DocName"]}", new Info
            //    {
            //        Title = Configuration["Swagger:Title"],
            //        Version = Configuration["Swagger:Version"]
            //    });
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    //app.UseHsts();
            //}

            //app.UseHttpsRedirection();
            //identityserver
            //app.UseAuthentication();
            //app.UseMvc();

            var url = Configuration["IdentityServer:Url"];
            var swaggerdoc = Configuration["swaggerdoc"];
            var swaggerdoc1 = Configuration["swaggerdoc1"];
                        
            var apilist = new Dictionary<string, string>();
            var iarray = 0;
            while (true) {
                if (Configuration[string.Format("apilist:{0}:dllname", iarray.ToString())] == null) {
                    break;
                }
                apilist.Add(Configuration[string.Format("apilist:{0}:dllname", iarray.ToString())], Configuration[string.Format("apilist:{0}:path", iarray.ToString())]);
                iarray++;
            };            

            app
               .UseSwagger()
               .UseSwaggerUI(options =>
               {
                   apilist.Keys.ToList().ForEach(key =>
                   {
                       options.SwaggerEndpoint($"/{key}/v1/swagger.json", $"{apilist[key]} -【{key}】");
                   });
                   options.DocumentTitle = "Swagger接口文档";
               });

            app.UseOcelot().Wait();
        }
    }
}
