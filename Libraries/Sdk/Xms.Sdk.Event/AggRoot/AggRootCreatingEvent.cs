using System.ComponentModel;
using Xms.Core.Data;
using Xms.Event.Abstractions;
using Xms.Sdk.Event.AggRoot;

namespace Xms.Sdk.Event
{
    [Description("聚合根创建前事件")]
    public class AggRootCreatingEvent : ObjectCreatingEvent<AggregateRoot>, IEvent
    {
        public AggRootCreatingEvent(AggregateRoot @object) : base(@object)
        {
            Object = @object;
        }

        public AggregateRootMetaData AggRootMetaData { get; set; }
    }
}