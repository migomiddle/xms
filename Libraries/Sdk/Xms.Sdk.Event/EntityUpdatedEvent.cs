using System.Collections.Generic;
using System.ComponentModel;
using Xms.Core.Data;
using Xms.Event.Abstractions;

namespace Xms.Sdk.Event
{
    [Description("实体更新后事件")]
    public class EntityUpdatedEvent : ObjectUpdatedEvent<Entity>, IEvent
    {
        public EntityUpdatedEvent(Entity origin, Entity updated) : base(origin, updated)
        {
            Origin = origin;
            Updated = updated;
        }

        public Schema.Domain.Entity EntityMetadata { get; set; }
        public List<Schema.Domain.Attribute> AttributeMetadatas { get; set; }
    }
}