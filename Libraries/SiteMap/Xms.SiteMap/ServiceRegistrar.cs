using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.SiteMap
{
    /// <summary>
    /// 菜单安全模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<SiteMap.IPrivilegeService, SiteMap.PrivilegeService>();
            services.AddScoped<SiteMap.IPrivilegeTreeBuilder, SiteMap.PrivilegeTreeBuilder>();
        }
    }
}