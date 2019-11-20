using System.Collections.Generic;
using System.ComponentModel;
using Xms.Core.Data;
using Xms.Event.Abstractions;

namespace Xms.Sdk.Event
{
    [Description("实体删除前事件")]
    public class EntityDeletingEvent : ObjectDeletingEvent<Entity>, IEvent
    {
        public EntityDeletingEvent(Entity @object) : base(@object)
        {
            Object = @object;
        }

        public Schema.Domain.Entity EntityMetadata { get; set; }
        public List<Schema.Domain.Attribute> AttributeMetadatas { get; set; }
    }
}