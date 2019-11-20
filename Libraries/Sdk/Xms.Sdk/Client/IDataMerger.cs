using System;
using System.Collections.Generic;

namespace Xms.Sdk.Client
{
    public interface IDataMerger
    {
        bool Merge(Guid entityId, Guid mainRecordId, Guid mergedRecordId, Dictionary<string, Guid> attributeMaps, bool ignorePermissions = false);
    }
}