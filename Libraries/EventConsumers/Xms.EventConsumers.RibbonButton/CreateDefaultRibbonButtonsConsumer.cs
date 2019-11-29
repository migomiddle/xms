using System;
using System.Linq;
using Xms.Event.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.RibbonButton;
using Xms.RibbonButton.Abstractions;
using Xms.Schema.Abstractions;

namespace Xms.EventConsumers.RibbonButton
{
    /// <summary>
    /// 实体创建事件消费者之创建默认按钮
    /// </summary>
    public class CreateDefaultRibbonButtonsConsumer : IConsumer<CreateDefaultButtonsEvent>
    {
        private readonly IRibbonButtonCreater _ribbonButtonCreater;
        private readonly IDefaultButtonProvider _defaultButtonProvider;

        public CreateDefaultRibbonButtonsConsumer(IRibbonButtonCreater ribbonButtonCreater, IDefaultButtonProvider defaultButtonProvider)
        {
            _ribbonButtonCreater = ribbonButtonCreater;
            _defaultButtonProvider = defaultButtonProvider;
        }

        public void HandleEvent(CreateDefaultButtonsEvent eventMessage)
        {
            if (eventMessage.DefaultButtons.NotEmpty())
            {
                var buttons = _defaultButtonProvider.Get(EntityMaskEnum.User)?.Where(x => eventMessage.DefaultButtons.Contains(x.RibbonButtonId)).ToList();
                if (buttons.NotEmpty())
                {
                    buttons.ForEach((b) =>
                    {
                        b.RibbonButtonId = Guid.NewGuid();
                        b.CreatedBy = eventMessage.Object.CreatedBy;
                        b.EntityId = eventMessage.Object.EntityId;
                    });
                    _ribbonButtonCreater.CreateMany(buttons);
                }
            }
        }
    }
}