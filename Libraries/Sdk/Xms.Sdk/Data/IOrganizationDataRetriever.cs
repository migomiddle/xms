using System.Collections.Generic;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Data
{
    public interface IOrganizationDataRetriever
    {
        PagedList<Entity> RetrievePaged(QueryBase request, bool ignorePermissions = false);

        IEnumerable<Entity> RetrieveAll(QueryBase request, bool ignorePermissions = false);

        Entity Retrieve(QueryBase request, bool ignorePermissions = false);
    }
}