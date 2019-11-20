using System;
using Xms.Core.Data;
using Xms.Sdk.Abstractions;

namespace Xms.Sdk.Client
{
    public interface IDataAssigner
    {
        bool Assign(Guid entityId, Guid recordId, OwnerObject owner, bool ignorePermissions = false);

        bool Assign(Schema.Domain.Entity entityMetadata, Entity entity, OwnerObject owner, bool ignorePermissions = false);
    }
}