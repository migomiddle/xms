using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xms.Infrastructure.Inject;

namespace Xms.Localization
{
    /// <summary>
    /// 本地化模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Localization.Abstractions.LocalizationOptions>(configuration.GetSection("AppConfig:Localization"));
            services.AddScoped<Localization.ILanguageService, Localization.LanguageService>();
            services.AddScoped<Localization.LocalizedTextXmlProvider>();
            services.AddScoped<Localization.LocalizedTextDbProvider>();
            //services.AddScoped<Localization.Abstractions.ILocalizedTextProvider, LocalizedTextXmlProvider>();
            services.AddScoped<Localization.Abstractions.ILocalizedTextProvider>((s) =>
            {
                if (s.GetService<IOptionsMonitor<Localization.Abstractions.LocalizationOptions>>().CurrentValue.Source == Abstractions.LocalizationSourceType.Xml)
                {
                    return s.GetService<LocalizedTextXmlProvider>();
                }
                return s.GetService<LocalizedTextDbProvider>();
            });
            services.AddScoped<Localization.ILocalizedLabelService, Localization.LocalizedLabelService>();
            services.AddScoped<Localization.ILocalizedLabelBatchBuilder, Localization.LocalizedLabelBatchBuilder>();
            services.AddScoped<Localization.ILocalizedLabelImportExport, Localization.LocalizedLabelImportExport>();
        }
    }
}