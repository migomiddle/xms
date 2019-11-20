using System.ComponentModel;

namespace Xms.Event.Abstractions
{
    [Description("对象删除后事件")]
    public class ObjectDeletedEvent<T> : IEvent
    {
        public ObjectDeletedEvent(string moduleName, T @object)
        {
            ModuleName = moduleName;
            Object = @object;
        }

        public T Object { get; set; }

        public string ModuleName { get; set; }
    }
}