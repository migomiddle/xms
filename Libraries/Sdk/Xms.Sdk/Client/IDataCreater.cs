using System;
using System.Collections.Generic;
using Xms.Core.Data;

namespace Xms.Sdk.Client
{
    public interface IDataCreater
    {
        Guid Create(Entity entity, bool ignorePermissions = false);

        bool CreateMany(IList<Entity> entities, bool ignorePermissions = false);
    }
}