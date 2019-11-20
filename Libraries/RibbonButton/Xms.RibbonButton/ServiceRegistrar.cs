using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.RibbonButton
{
    /// <summary>
    /// 按钮模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<RibbonButton.IRibbonButtonCreater, RibbonButton.RibbonButtonCreater>();
            services.AddScoped<RibbonButton.IRibbonButtonUpdater, RibbonButton.RibbonButtonUpdater>();
            services.AddScoped<RibbonButton.IRibbonButtonFinder, RibbonButton.RibbonButtonFinder>();
            services.AddScoped<RibbonButton.IRibbonButtonDeleter, RibbonButton.RibbonButtonDeleter>();
            services.AddScoped<RibbonButton.IRibbonButtonImporter, RibbonButton.RibbonButtonImporter>();
            services.AddScoped<RibbonButton.IRibbonButtonStatusSetter, RibbonButton.RibbonButtonStatusSetter>();
            services.AddScoped<RibbonButton.IDefaultButtonProvider, RibbonButton.DefaultButtonProvider>();
            services.AddScoped<RibbonButton.IRibbonButtonDependency, RibbonButton.RibbonButtonDependency>();
        }
    }
}