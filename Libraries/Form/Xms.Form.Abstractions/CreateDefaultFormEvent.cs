using Xms.Event.Abstractions;

namespace Xms.Form.Abstractions
{
    /// <summary>
    /// 创建默认表单事件
    /// </summary>
    public class CreateDefaultFormEvent : ObjectCreatedEvent<Schema.Domain.Entity>
    {
        public CreateDefaultFormEvent(Schema.Domain.Entity entity) : base(entity)
        {
            Object = entity;
        }
    }
}