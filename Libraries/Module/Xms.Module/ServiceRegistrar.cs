using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Module
{
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Module.Data.IModuleRepository, Module.Data.ModuleRepository>();
            services.AddScoped<Module.IModuleService, Module.ModuleService>();
            services.AddScoped<Module.Abstractions.IModuleRegistrar, ModuleRegistrar>();
        }
    }
}