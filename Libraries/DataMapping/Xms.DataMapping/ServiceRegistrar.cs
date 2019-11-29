using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.DataMapping
{
    /// <summary>
    /// 数据映射模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<DataMapping.IEntityMapCreater, DataMapping.EntityMapCreater>();
            services.AddScoped<DataMapping.IEntityMapUpdater, DataMapping.EntityMapUpdater>();
            services.AddScoped<DataMapping.IEntityMapDeleter, DataMapping.EntityMapDeleter>();
            services.AddScoped<DataMapping.IEntityMapFinder, DataMapping.EntityMapFinder>();
            services.AddScoped<DataMapping.IAttributeMapCreater, DataMapping.AttributeMapCreater>();
            services.AddScoped<DataMapping.IAttributeMapUpdater, DataMapping.AttributeMapUpdater>();
            services.AddScoped<DataMapping.IAttributeMapDeleter, DataMapping.AttributeMapDeleter>();
            services.AddScoped<DataMapping.IAttributeMapFinder, DataMapping.AttributeMapFinder>();
            services.AddScoped<DataMapping.IEntityMapDependency, DataMapping.EntityMapDependency>();
        }
    }
}