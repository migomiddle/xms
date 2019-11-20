using System.Collections.Generic;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Client
{
    public interface IDataFinder
    {
        Entity Retrieve(QueryBase request, bool ignorePermissions = false);

        List<Entity> RetrieveAll(QueryBase request, bool ignorePermissions = false);

        PagedList<Entity> RetrieveMultiple(QueryBase request, bool ignorePermissions = false);
    }
}