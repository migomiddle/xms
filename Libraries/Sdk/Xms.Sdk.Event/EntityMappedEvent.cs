using System.ComponentModel;
using Xms.Core.Data;
using Xms.Event.Abstractions;

namespace Xms.Sdk.Event
{
    [Description("实体转换后事件")]
    public class EntityMappedEvent : IEvent
    {
        public Entity Target { get; set; }
        public Entity Source { get; set; }
        public Schema.Domain.Entity EntityMetadata { get; set; }
    }
}