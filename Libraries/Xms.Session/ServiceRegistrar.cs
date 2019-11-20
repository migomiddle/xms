using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Session
{
    /// <summary>
    /// session模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Session.ISessionService, Session.SessionService>();
            services.AddScoped<Session.ISession, Session.AspNetSession>();
        }
    }
}