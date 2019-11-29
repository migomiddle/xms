using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Logging.AppLog
{
    /// <summary>
    /// 应用程序日志模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Logging.AppLog.ILogService, Logging.AppLog.LogService>();
        }
    }
}