using System.ComponentModel;

namespace Xms.Event.Abstractions
{
    [Description("对象创建前事件")]
    public class ObjectCreatingEvent<T> : IEvent
    {
        public ObjectCreatingEvent(T @object)
        {
            Object = @object;
        }

        public T Object { get; set; }
    }
}