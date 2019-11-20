using Xms.Plugin.Domain;
using Xms.Core;
using Xms.Core.Data;
using Xms.Plugin.Abstractions;
using System.Collections.Generic;
using System;

namespace Xms.Plugin
{
    public interface IPluginExecutor<TData, KMetadata>
    {
        void Execute(Guid entityId,Guid? businessObjectId, PlugInType typeCode, OperationTypeEnum op,OperationStage stage, TData tData, KMetadata kMetadata);
        IPlugin<TData, KMetadata> GetInstance(EntityPlugin entity);
    }
}