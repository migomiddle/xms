using Xms.Core.Data;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Client.AggRoot
{
    public interface IAggFinder
    {
        AggregateRoot Retrieve(QueryBase request, bool ignorePermissions = false);
    }
}