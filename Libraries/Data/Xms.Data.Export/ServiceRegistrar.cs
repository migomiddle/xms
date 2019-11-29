using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Data.Export
{
    /// <summary>
    /// 数据导出模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Data.Export.IDataExporter, Data.Export.DataExporter>();
        }
    }
}