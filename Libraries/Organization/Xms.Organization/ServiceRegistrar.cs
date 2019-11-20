using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Organization
{
    /// <summary>
    /// 组织模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Organization.Data.IOrganizationBaseRepository, Organization.Data.OrganizationBaseRepository>();
            services.AddScoped<Organization.Data.ISystemUserSettingsRepository, Organization.Data.SystemUserSettingsRepository>();
            services.AddScoped<Organization.IOrganizationBaseService, Organization.OrganizationBaseService>();
            services.AddScoped<Organization.IOrganizationService, Organization.OrganizationService>();
            services.AddScoped<Organization.IBusinessUnitService, Organization.BusinessUnitService>();
            services.AddScoped<Organization.ISystemUserService, Organization.SystemUserService>();
        }
    }
}