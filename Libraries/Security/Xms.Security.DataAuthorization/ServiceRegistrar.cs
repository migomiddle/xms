using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Security.DataAuthorization
{
    /// <summary>
    /// 数据授权模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Authorization.Abstractions.IPrincipalObjectAccessService, Security.DataAuthorization.PrincipalObjectAccessService>();
            services.AddScoped<Security.DataAuthorization.IEntityPermissionService, Security.DataAuthorization.EntityPermissionService>();
            services.AddScoped<Authorization.Abstractions.IRoleObjectAccessService, Security.DataAuthorization.RoleObjectAccessService>();
            services.AddScoped<Authorization.Abstractions.IRoleObjectAccessEntityPermissionService, Security.DataAuthorization.RoleObjectAccessEntityPermissionService>();
        }
    }
}