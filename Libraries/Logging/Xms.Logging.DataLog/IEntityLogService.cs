using System;
using System.Collections.Generic;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Logging.DataLog.Domain;
using Xms.Sdk.Abstractions;

namespace Xms.Logging.DataLog
{
    public interface IEntityLogService
    {
        #region sync

        bool Create(EntityLog entity);

        bool CreatedLog(Entity data, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas);

        bool UpdatedLog(Entity originData, Entity newData, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas);

        bool DeletedLog(Entity data, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas);

        bool SharedLog(Entity data, Schema.Domain.Entity entityMetadata);

        bool AssignedLog(Entity data, OwnerObject from, OwnerObject to, Schema.Domain.Entity entityMetadata);

        bool MergedLog(Entity merged, Entity target, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas);

        EntityLog FindById(Guid id);

        List<EntityLog> Query(Func<QueryDescriptor<EntityLog>, QueryDescriptor<EntityLog>> container);

        PagedList<EntityLog> QueryPaged(Func<QueryDescriptor<EntityLog>, QueryDescriptor<EntityLog>> container);

        void Clear();

        void Clear(Guid entityId);

        #endregion sync
    }
}