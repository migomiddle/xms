using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Configuration
{
    /// <summary>
    /// 参数配置模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Configuration.ISettingService, Configuration.SettingService>();
            services.AddScoped<Configuration.ISettingFinder, Configuration.SettingFinder>();
        }
    }
}