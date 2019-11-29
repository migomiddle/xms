using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Business.DuplicateValidator
{
    /// <summary>
    /// 数据验证模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Business.DuplicateValidator.IDuplicateRuleCreater, Business.DuplicateValidator.DuplicateRuleCreater>();
            services.AddScoped<Business.DuplicateValidator.IDuplicateRuleUpdater, Business.DuplicateValidator.DuplicateRuleUpdater>();
            services.AddScoped<Business.DuplicateValidator.IDuplicateRuleFinder, Business.DuplicateValidator.DuplicateRuleFinder>();
            services.AddScoped<Business.DuplicateValidator.IDuplicateRuleDeleter, Business.DuplicateValidator.DuplicateRuleDeleter>();
            services.AddScoped<Business.DuplicateValidator.IDuplicateRuleExecutor, Business.DuplicateValidator.DuplicateRuleExecutor>();
            services.AddScoped<Business.DuplicateValidator.IDuplicateRuleConditionService, Business.DuplicateValidator.DuplicateRuleConditionService>();
            services.AddScoped<Business.DuplicateValidator.IDuplicateRuleDependency, Business.DuplicateValidator.DuplicateRuleDependency>();
        }
    }
}