using System.ComponentModel;
using Xms.Core.Data;
using Xms.Event.Abstractions;

namespace Xms.Sdk.Event
{
    [Description("实体分派前事件")]
    public class EntityAssigningEvent : IEvent
    {
        public Entity OriginData { get; set; }
        public Entity Data { get; set; }
        public Schema.Domain.Entity EntityMetadata { get; set; }
    }
}