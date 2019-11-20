using System.Collections.Generic;
using System.ComponentModel;
using Xms.Core.Data;
using Xms.Event.Abstractions;

namespace Xms.Sdk.Event
{
    [Description("实体更新前事件")]
    public class EntityUpdatingEvent : ObjectUpdatingEvent<Entity>, IEvent
    {
        public EntityUpdatingEvent(Entity origin, Entity updated) : base(origin, updated)
        {
            Origin = origin;
            Updated = updated;
        }

        public Schema.Domain.Entity EntityMetadata { get; set; }
        public List<Schema.Domain.Attribute> AttributeMetadatas { get; set; }
    }
}