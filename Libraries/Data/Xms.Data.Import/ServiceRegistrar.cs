using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Data.Import
{
    /// <summary>
    /// 数据导入模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDataImporter, DataImporter>();
            services.AddScoped<IImportDataService, ImportDataService>();
            services.AddScoped<IImportFileService, ImportFileService>();
            services.AddScoped<IImportMapService, ImportMapService>();
            services.AddScoped<IFileTemplateProvider, FileTemplateProvider>();
        }
    }
}