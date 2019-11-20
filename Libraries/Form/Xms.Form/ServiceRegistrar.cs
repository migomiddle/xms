using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Form
{
    /// <summary>
    /// 表单模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Form.ISystemFormCreater, Form.SystemFormCreater>();
            services.AddScoped<Form.ISystemFormDeleter, Form.SystemFormDeleter>();
            services.AddScoped<Form.ISystemFormFinder, Form.SystemFormFinder>();
            services.AddScoped<Form.ISystemFormUpdater, Form.SystemFormUpdater>();
            services.AddScoped<Form.ISystemFormImporter, Form.SystemFormImporter>();
            services.AddScoped<Form.IDefaultSystemFormProvider, Form.DefaultSystemFormProvider>();
            services.AddScoped<Form.IFormService, Form.FormService>();
            services.AddScoped<Form.ISystemFormDependency, Form.SystemFormDependency>();
        }
    }
}