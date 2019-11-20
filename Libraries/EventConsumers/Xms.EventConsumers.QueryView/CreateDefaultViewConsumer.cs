using Xms.Event.Abstractions;
using Xms.QueryView;
using Xms.QueryView.Abstractions;

namespace Xms.EventConsumers.QueryView
{
    /// <summary>
    /// 事件消费者之创建默认视图
    /// </summary>
    public class CreateDefaultViewConsumer : IConsumer<CreateDefaultViewEvent>
    {
        private readonly IQueryViewCreater _queryViewCreater;

        public CreateDefaultViewConsumer(IQueryViewCreater queryViewCreater)
        {
            _queryViewCreater = queryViewCreater;
        }

        public void HandleEvent(CreateDefaultViewEvent eventMessage)
        {
            _queryViewCreater.CreateDefaultView(eventMessage.Object);
        }
    }
}