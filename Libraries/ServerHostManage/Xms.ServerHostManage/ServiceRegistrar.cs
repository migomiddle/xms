using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.ServerHostManage
{
    /// <summary>
    /// ServerHostManage服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ServerHostManage.IServerHostManageService, ServerHostManage.ServerHostManageService>();
        }
    }
}