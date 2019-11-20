using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Query
{
    public interface IQueryResolverFactory
    {
        IQueryResolver Get(QueryBase query);
    }
}