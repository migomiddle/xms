using System.ComponentModel;
using Xms.Core.Data;
using Xms.Event.Abstractions;
using Xms.Sdk.Event.AggRoot;

namespace Xms.Sdk.Event
{
    [Description("聚合根创建后事件")]
    public class AggRootCreatedEvent : ObjectCreatedEvent<AggregateRoot>, IEvent
    {
        public AggRootCreatedEvent(AggregateRoot @object) : base(@object)
        {
            Object = @object;
        }

        public AggregateRootMetaData AggRootMetaData { get; set; }
    }
}