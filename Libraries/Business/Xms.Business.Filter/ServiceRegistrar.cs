using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Business.Filter
{
    /// <summary>
    /// 拦截模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Business.Filter.IFilterRuleCreater, Business.Filter.FilterRuleCreater>();
            services.AddScoped<Business.Filter.IFilterRuleUpdater, Business.Filter.FilterRuleUpdater>();
            services.AddScoped<Business.Filter.IFilterRuleFinder, Business.Filter.FilterRuleFinder>();
            services.AddScoped<Business.Filter.IFilterRuleDeleter, Business.Filter.FilterRuleDeleter>();
            services.AddScoped<Business.Filter.IFilterRuleExecutor, Business.Filter.FilterRuleExecutor>();
            services.AddScoped<Business.Filter.IFilterRuleDependency, Business.Filter.FilterRuleDependency>();
        }
    }
}