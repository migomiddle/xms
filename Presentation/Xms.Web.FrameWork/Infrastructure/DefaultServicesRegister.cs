using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Context;
using Xms.Core;
using Xms.Core.Org;
using Xms.Identity;
using Xms.Infrastructure;
using Xms.Infrastructure.Inject;
using Xms.Infrastructure.Utility;
using Xms.Organization;
using Xms.Web.Framework.Context;

namespace Xms.Web.Framework.Infrastructure
{
    /// <summary>
    /// 服务注入
    /// </summary>
    public class DefaultServicesRegister : IServiceRegistrar
    {
        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            //服务解析器
            services.AddScoped<IServiceResolver, ServiceResolver>();
            //web上下文
            services.AddScoped<IAppContext, WebAppContext>();
            services.AddScoped<IWebAppContext, WebAppContext>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IOrgDataServer, OrgDataServer>();
            services.AddScoped<IWebHelper, WebHelper>();

            //exception handler
            services.AddScoped<IExceptionHandlerFactory, ExceptionHandlerFactory>();
            services.RegisterScope(typeof(IExceptionHandler<>));
        }

        public int Order => 0;
    }
}