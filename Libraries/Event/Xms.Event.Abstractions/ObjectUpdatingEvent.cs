using System.ComponentModel;

namespace Xms.Event.Abstractions
{
    [Description("对象更新前事件")]
    public class ObjectUpdatingEvent<T> : IEvent
    {
        public ObjectUpdatingEvent(T origin, T updated)
        {
            Origin = origin;
            Updated = updated;
        }

        public T Origin { get; set; }
        public T Updated { get; set; }
    }
}