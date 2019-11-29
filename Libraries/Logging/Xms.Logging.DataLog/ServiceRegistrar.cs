using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Logging.DataLog
{
    /// <summary>
    /// 数据日志模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Logging.DataLog.IEntityLogService, Logging.DataLog.EntityLogService>();
        }
    }
}