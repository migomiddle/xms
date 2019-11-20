using System.Collections.Generic;
using Xms.Core.Data;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Client
{
    public interface IDataUpdater
    {
        bool Update(Entity entity, bool ignorePermissions = false);

        bool Update(Entity entity, QueryExpression query, bool ignorePermissions = false);

        bool Update(IList<Entity> entities, bool ignorePermissions = false);
    }
}