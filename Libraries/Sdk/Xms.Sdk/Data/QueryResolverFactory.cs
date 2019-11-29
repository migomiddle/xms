using Xms.Infrastructure.Inject;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Query;

namespace Xms.Sdk.Data
{
    /// <summary>
    /// 查询表达式解析工厂
    /// </summary>
    public class QueryResolverFactory : IQueryResolverFactory
    {
        //private readonly IAppContext _appContext;
        //private readonly IDbContext _dbContext;
        //private readonly IQueryMetadataFinder _queryMetadataFinder;
        //private readonly IAttributeFinder _attributeFinder;
        private readonly IServiceResolver _serviceResolver;

        public QueryResolverFactory(
            //IAppContext appContext
            //, IDbContext dbContext
            //, IQueryMetadataFinder queryMetadataFinder
            //, IAttributeFinder attributeFinder
            //,
            IServiceResolver serviceResolver
            )
        {
            //_appContext = appContext;
            //_dbContext = dbContext;
            //_queryMetadataFinder = queryMetadataFinder;
            //_attributeFinder = attributeFinder;
            _serviceResolver = serviceResolver;
        }

        public IQueryResolver Get(QueryBase query)
        {
            if (query is QueryExpression)
            {
                return _serviceResolver.Get<QueryExpressionResolver>().Init(query as QueryExpression);
                //return new QueryExpressionResolver(_appContext, _dbContext, _queryMetadataFinder, _attributeFinder).Init(query as QueryExpression);
            }
            else if (query is QueryByAttribute)
            {
                return _serviceResolver.Get<QueryByAttributeResolver>().Init(query as QueryByAttribute);
                //return new QueryByAttributeResolver(_appContext, _dbContext, _queryMetadataFinder).Init(query as QueryByAttribute);
            }
            return null;
        }
    }
}