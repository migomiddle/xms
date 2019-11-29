using System.Collections.Generic;
using System.ComponentModel;
using Xms.Core.Data;
using Xms.Event.Abstractions;

namespace Xms.Sdk.Event
{
    [Description("实体删除后事件")]
    public class EntityDeletedEvent : ObjectDeletedEvent<Entity>, IEvent
    {
        public EntityDeletedEvent(Entity @object) : base("", @object)
        {
            Object = @object;
        }

        public Entity Data { get; set; }
        public Schema.Domain.Entity EntityMetadata { get; set; }
        public List<Schema.Domain.Attribute> AttributeMetadatas { get; set; }
    }
}