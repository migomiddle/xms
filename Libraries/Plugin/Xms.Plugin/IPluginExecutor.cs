using System;
using Xms.Core;
using Xms.Plugin.Abstractions;
using Xms.Plugin.Domain;

namespace Xms.Plugin
{
    public interface IPluginExecutor<TData, KMetadata>
    {
        void Execute(Guid entityId, Guid? businessObjectId, PlugInType typeCode, OperationTypeEnum op, OperationStage stage, TData tData, KMetadata kMetadata);

        IPlugin<TData, KMetadata> GetInstance(EntityPlugin entity);
    }
}