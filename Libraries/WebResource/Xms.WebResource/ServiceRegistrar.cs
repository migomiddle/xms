using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.WebResource
{
    /// <summary>
    /// web资源模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<WebResource.IWebResourceCreater, WebResource.WebResourceCreater>();
            services.AddScoped<WebResource.IWebResourceUpdater, WebResource.WebResourceUpdater>();
            services.AddScoped<WebResource.IWebResourceFinder, WebResource.WebResourceFinder>();
            services.AddScoped<WebResource.IWebResourceDeleter, WebResource.WebResourceDeleter>();
            services.AddScoped<WebResource.IWebResourceContentCoder, WebResource.WebResourceContentCoder>();
        }
    }
}