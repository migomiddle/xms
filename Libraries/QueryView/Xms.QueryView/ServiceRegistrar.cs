using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.QueryView
{
    /// <summary>
    /// 视图模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<QueryView.IQueryViewCreater, QueryView.QueryViewCreater>();
            services.AddScoped<QueryView.IQueryViewDeleter, QueryView.QueryViewDeleter>();
            services.AddScoped<QueryView.IQueryViewFinder, QueryView.QueryViewFinder>();
            services.AddScoped<QueryView.IQueryViewUpdater, QueryView.QueryViewUpdater>();
            services.AddScoped<QueryView.IQueryViewImporter, QueryView.QueryViewImporter>();
            services.AddScoped<QueryView.IDefaultQueryViewProvider, QueryView.DefaultQueryViewProvider>();
            services.AddScoped<QueryView.IGridService, QueryView.GridService>();
            services.AddScoped<QueryView.IQueryViewDependency, QueryView.QueryViewDependency>();
        }
    }
}