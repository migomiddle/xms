using System.ComponentModel;
using Xms.Core.Data;
using Xms.Event.Abstractions;
using Xms.Sdk.Event.AggRoot;

namespace Xms.Sdk.Event
{
    [Description("聚合根删除前事件")]
    public class AggRootDeletingEvent : ObjectDeletingEvent<AggregateRoot>, IEvent
    {
        public AggRootDeletingEvent(AggregateRoot @object) : base(@object)
        {
            Object = @object;
        }

        public AggregateRootMetaData AggRootMetaData { get; set; }
    }
}