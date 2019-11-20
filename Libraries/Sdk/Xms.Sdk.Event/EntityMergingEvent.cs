using System.ComponentModel;
using Xms.Core.Data;
using Xms.Event.Abstractions;

namespace Xms.Sdk.Event
{
    [Description("实体合并前事件")]
    public class EntityMergingEvent : IEvent
    {
        public Entity Target { get; set; }
        public Entity Merged { get; set; }
        public Schema.Domain.Entity EntityMetadata { get; set; }
    }
}