using System.ComponentModel;

namespace Xms.Event.Abstractions
{
    [Description("对象更新后事件")]
    public class ObjectUpdatedEvent<T> : IEvent
    {
        public ObjectUpdatedEvent(T origin, T updated)
        {
            Origin = origin;
            Updated = updated;
        }

        public T Origin { get; set; }
        public T Updated { get; set; }
    }
}