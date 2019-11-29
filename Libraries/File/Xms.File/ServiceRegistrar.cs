using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.File
{
    /// <summary>
    /// 文件模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<File.IAttachmentCreater, File.AttachmentCreater>();
            services.AddScoped<File.IAttachmentFinder, File.AttachmentFinder>();
            services.AddScoped<File.IAttachmentDeleter, File.AttachmentDeleter>();
        }
    }
}