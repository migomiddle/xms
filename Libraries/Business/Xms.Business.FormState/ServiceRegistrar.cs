using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Business.FormStateRule
{
    /// <summary>
    /// 表单状态规则模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Business.FormStateRule.ISystemFormStateRuleService, Business.FormStateRule.SystemFormStateRuleService>();
            services.AddScoped<Business.FormStateRule.ISystemFormStatusSetter, Business.FormStateRule.SystemFormStatusSetter>();
        }
    }
}