using System;
using System.Collections.Generic;
using Xms.Core.Data;

namespace Xms.Sdk.Client
{
    public interface IDataDeleter
    {
        bool Delete(Entity record, bool ignorePermissions = false);

        bool Delete(string name, Guid id, bool ignorePermissions = false);

        bool Delete(string name, IEnumerable<Guid> ids, bool ignorePermissions = false);
    }
}