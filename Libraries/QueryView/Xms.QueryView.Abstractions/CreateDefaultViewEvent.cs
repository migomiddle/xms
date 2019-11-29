using Xms.Event.Abstractions;

namespace Xms.QueryView.Abstractions
{
    /// <summary>
    /// 创建默认视图事件
    /// </summary>
    public class CreateDefaultViewEvent : ObjectCreatedEvent<Schema.Domain.Entity>
    {
        public CreateDefaultViewEvent(Schema.Domain.Entity entity) : base(entity)
        {
            Object = entity;
        }
    }
}