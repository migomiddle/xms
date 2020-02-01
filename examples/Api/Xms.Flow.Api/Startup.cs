using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;
using Xms.Api.Core;
using Xms.Api.Core.Swagger;
using Xms.Web.Framework.Infrastructure;

namespace Xms.Flow.Api
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
            services.AddXmsCors();
            //默认web服务
            services.AddWebDefaults(Configuration);
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
            //swagger ui
            app.UseXmsSwaggerUI();
            //默认web配置
            app.UseWebDefaults();
        }
    }

    public class StartupPart : StartupBase, IStartupPart
    {
        public StartupPart(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //支持跨域
            services.AddXmsCors();
            //默认web服务
            services.AddWebDefaults(Configuration);
            //swagger
            services.AddXmsSwaggerGen();
        }
        public override void Configure(IApplicationBuilder app)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //swagger ui
            app.UseXmsSwaggerUI();
            //默认web配置
            app.UseWebDefaults();
        }
    }
}
