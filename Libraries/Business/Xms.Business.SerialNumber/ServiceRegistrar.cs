using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Business.SerialNumber
{
    /// <summary>
    /// 自动编号模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Business.SerialNumber.ISerialNumberRuleCreater, Business.SerialNumber.SerialNumberRuleCreater>();
            services.AddScoped<Business.SerialNumber.ISerialNumberRuleUpdater, Business.SerialNumber.SerialNumberRuleUpdater>();
            services.AddScoped<Business.SerialNumber.ISerialNumberRuleDeleter, Business.SerialNumber.SerialNumberRuleDeleter>();
            services.AddScoped<Business.SerialNumber.ISerialNumberRuleFinder, Business.SerialNumber.SerialNumberRuleFinder>();
            services.AddScoped<Business.SerialNumber.ISerialNumberGenerator, Business.SerialNumber.SerialNumberGenerator>();
            services.AddScoped<Business.SerialNumber.IVariableReplacer, Business.SerialNumber.DateTimeVariableReplacer>();
            services.AddScoped<Business.SerialNumber.IVariableReplacer, Business.SerialNumber.UserVariableReplacer>();
            services.AddScoped<Business.SerialNumber.ISerialNumberDependency, Business.SerialNumber.SerialNumberDependency>();
        }
    }
}