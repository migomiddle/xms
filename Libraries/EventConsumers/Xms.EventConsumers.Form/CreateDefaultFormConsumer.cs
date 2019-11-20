using Xms.Event.Abstractions;
using Xms.Form;
using Xms.Form.Abstractions;

namespace Xms.EventConsumers.Form
{
    /// <summary>
    /// 事件消费者之创建默认表单
    /// </summary>
    public class CreateDefaultFormConsumer : IConsumer<CreateDefaultFormEvent>
    {
        private readonly ISystemFormCreater _systemFormCreater;

        public CreateDefaultFormConsumer(ISystemFormCreater systemFormCreater)
        {
            _systemFormCreater = systemFormCreater;
        }

        public void HandleEvent(CreateDefaultFormEvent eventMessage)
        {
            _systemFormCreater.CreateDefaultForm(eventMessage.Object);
        }
    }
}