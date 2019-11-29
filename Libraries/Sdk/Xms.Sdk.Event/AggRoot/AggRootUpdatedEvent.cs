using System.ComponentModel;
using Xms.Core.Data;
using Xms.Event.Abstractions;
using Xms.Sdk.Event.AggRoot;

namespace Xms.Sdk.Event
{
    [Description("聚合根更新后事件")]
    public class AggRootUpdatedEvent : ObjectUpdatedEvent<AggregateRoot>, IEvent
    {
        public AggRootUpdatedEvent(AggregateRoot origin, AggregateRoot updated) : base(origin, updated)
        {
            Origin = origin;
            Updated = updated;
        }

        public AggregateRootMetaData AggRootMetaData { get; set; }
    }
}