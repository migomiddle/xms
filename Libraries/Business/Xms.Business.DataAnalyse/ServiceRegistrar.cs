using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Business.DataAnalyse
{
    /// <summary>
    /// 数据分析模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Business.DataAnalyse.Visualization.IChartCreater, Business.DataAnalyse.Visualization.ChartCreater>();
            services.AddScoped<Business.DataAnalyse.Visualization.IChartUpdater, Business.DataAnalyse.Visualization.ChartUpdater>();
            services.AddScoped<Business.DataAnalyse.Visualization.IChartFinder, Business.DataAnalyse.Visualization.ChartFinder>();
            services.AddScoped<Business.DataAnalyse.Visualization.IChartDeleter, Business.DataAnalyse.Visualization.ChartDeleter>();
            services.AddScoped<Business.DataAnalyse.Visualization.IChartBuilder, Business.DataAnalyse.Visualization.ChartBuilder>();
            services.AddScoped<Business.DataAnalyse.Visualization.IChartDependency, Business.DataAnalyse.Visualization.ChartDependency>();
            services.AddScoped<Business.DataAnalyse.Report.IReportService, Business.DataAnalyse.Report.ReportService>();
        }
    }
}