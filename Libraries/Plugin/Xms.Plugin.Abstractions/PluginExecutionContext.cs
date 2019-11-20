using System.Collections.Generic;
using Xms.Core;
using Xms.Core.Context;
using Xms.Core.Data;

namespace Xms.Plugin.Abstractions
{
    public class PluginExecutionContext
    {
        public OperationTypeEnum MessageName { get; set; }
        public Entity Target { get; set; }

        public OperationStage Stage { get; set; }

        public IUserContext User { get; set; }
        public Schema.Domain.Entity EntityMetadata { get; set; }
        public List<Schema.Domain.Attribute> AttributeMetadatas { get; set; }
    }
}