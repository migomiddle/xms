using System.ComponentModel;
using Xms.Core.Data;
using Xms.Event.Abstractions;
using Xms.Sdk.Event.AggRoot;

namespace Xms.Sdk.Event
{
    [Description("聚合根删除后事件")]
    public class AggRootDeletedEvent : ObjectDeletedEvent<AggregateRoot>, IEvent
    {
        public AggRootDeletedEvent(AggregateRoot @object) : base("", @object)
        {
            Object = @object;
        }

        public AggregateRootMetaData AggRootMetaData { get; set; }
    }
}