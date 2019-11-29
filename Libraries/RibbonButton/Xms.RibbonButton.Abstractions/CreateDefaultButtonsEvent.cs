using System;
using Xms.Event.Abstractions;

namespace Xms.RibbonButton.Abstractions
{
    /// <summary>
    /// 创建默认按钮
    /// </summary>
    public class CreateDefaultButtonsEvent : ObjectCreatedEvent<Schema.Domain.Entity>
    {
        public CreateDefaultButtonsEvent(Schema.Domain.Entity entity) : base(entity)
        {
            Object = entity;
        }

        public CreateDefaultButtonsEvent(Schema.Domain.Entity entity, Guid[] defaultButtons) : base(entity)
        {
            Object = entity;
            DefaultButtons = defaultButtons;
        }

        public Guid[] DefaultButtons { get; set; }
    }
}