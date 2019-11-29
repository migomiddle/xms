using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Sdk
{
    /// <summary>
    /// sdk服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Sdk.Data.IOrganizationDataProvider, Sdk.Data.OrganizationDataProvider>();
            services.AddScoped<Sdk.Data.IOrganizationDataRetriever, Sdk.Data.OrganizationDataRetriever>();
            services.AddScoped<Sdk.Data.IFieldValueUpdater, Sdk.Data.FieldValueUpdater>();
            services.AddScoped<Sdk.Query.IAggregateExpressionResolver, Sdk.Data.AggregateExpressionResolver>();
            services.AddScoped<Sdk.Client.IFetchDataService, Sdk.Client.FetchDataService>();
            services.AddScoped<Sdk.Client.IAggregateService, Sdk.Client.AggregateService>();
            services.AddScoped<Sdk.Client.IDataCreater, Sdk.Client.DataCreater>();
            services.AddScoped<Sdk.Client.IDataUpdater, Sdk.Client.DataUpdater>();
            services.AddScoped<Sdk.Client.IDataFinder, Sdk.Client.DataFinder>();
            services.AddScoped<Sdk.Client.IDataDeleter, Sdk.Client.DataDeleter>();
            services.AddScoped<Sdk.Client.IDataSharer, Sdk.Client.DataSharer>();
            services.AddScoped<Sdk.Client.IDataAssigner, Sdk.Client.DataAssigner>();
            services.AddScoped<Sdk.Client.IDataMerger, Sdk.Client.DataMerger>();
            services.AddScoped<Sdk.Client.IMapUpdater, Sdk.Client.MapUpdater>();
            services.AddScoped<Sdk.Client.IFormulaUpdater, Sdk.Client.FormulaUpdater>();
            services.AddScoped<Sdk.Client.IDataMapper, Sdk.Client.DataMapper>();
            services.AddScoped<Sdk.Client.IEntityValidator, Sdk.Client.EntityValidator>();
            services.AddScoped<Sdk.Extensions.IQueryMetadataFinder, Sdk.Extensions.QueryMetadataFinder>();
            services.AddScoped<Sdk.Query.IQueryResolverFactory, Sdk.Data.QueryResolverFactory>();

            services.AddScoped<Sdk.Client.AggRoot.IAggCreater, Sdk.Client.AggRoot.AggCreater>();
            services.AddScoped<Sdk.Client.AggRoot.IAggUpdater, Sdk.Client.AggRoot.AggUpdater>();
            services.AddScoped<Sdk.Client.AggRoot.IAggFinder, Sdk.Client.AggRoot.AggFinder>();

            //services.AddTransient<Sdk.Query.IQueryResolver, Sdk.Data.QueryExpressionResolver>();
            services.AddTransient<Sdk.Data.QueryExpressionResolver>();
            services.AddTransient<Sdk.Data.QueryByAttributeResolver>();
        }
    }
}