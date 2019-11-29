using System.Collections.Generic;
using Xms.Core;
using Xms.Core.Data;
using Xms.Plugin.Abstractions;
using Xms.Plugin.Domain;

namespace Xms.Plugin
{
    public interface IEntityPluginExecutor
    {
        void Execute(OperationTypeEnum op, OperationStage stage, Entity entity, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas);

        IEntityPlugin GetInstance(EntityPlugin entity);
    }
}