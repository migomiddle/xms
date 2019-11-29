using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Core;
using Xms.Infrastructure.Inject;

namespace Xms.Dependency
{
    /// <summary>
    /// 依赖关系服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Dependency.IDependencyService, Dependency.DependencyService>();
            services.AddScoped<Dependency.IDependencyBatchBuilder, Dependency.DependencyBatchBuilder>();
            //dependency component
            services.AddScoped<Dependency.IDependencyLookupFactory, Dependency.DependencyLookupFactory>();
            services.AddScoped<Dependency.IDependencyChecker, Dependency.DependencyChecker>();
            services.RegisterScope(typeof(Dependency.Abstractions.IDependentLookup<>));
            services.RegisterScope<Dependency.Abstractions.IDependentLookup>();
        }
    }
}