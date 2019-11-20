using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Security.Principal
{
    /// <summary>
    /// 安全模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Security.Principal.Data.ISystemUserPermissionRepository, Security.Principal.Data.SystemUserPermissionRepository>();
            services.AddScoped<Security.Principal.IPermissionService, Security.Principal.PermissionService>();
            services.AddScoped<Security.Principal.ISystemUserRolesService, Security.Principal.SystemUserRolesService>();
            services.AddScoped<Security.Principal.ISystemUserPermissionService, Security.Principal.SystemUserPermissionService>();
        }
    }
}