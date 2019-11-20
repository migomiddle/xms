using System.ComponentModel;

namespace Xms.Event.Abstractions
{
    [Description("对象创建后事件")]
    public class ObjectCreatedEvent<T> : IEvent
    {
        public ObjectCreatedEvent(T @object)
        {
            Object = @object;
        }

        public T Object { get; set; }
    }
}