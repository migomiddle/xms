using System.ComponentModel;
using Xms.Core.Data;
using Xms.Event.Abstractions;
using Xms.Sdk.Event.AggRoot;

namespace Xms.Sdk.Event
{
    [Description("聚合根更新前事件")]
    public class AggRootUpdatingEvent : ObjectUpdatingEvent<AggregateRoot>, IEvent
    {
        public AggRootUpdatingEvent(AggregateRoot origin, AggregateRoot updated) : base(origin, updated)
        {
            Origin = origin;
            Updated = updated;
        }

        public AggregateRootMetaData AggRootMetaData { get; set; }
    }
}