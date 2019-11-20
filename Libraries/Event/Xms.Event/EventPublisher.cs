using System;
using System.Linq;
using Xms.Event.Abstractions;
using Xms.Infrastructure.Inject;
using Xms.Logging.AppLog;

namespace Xms.Event
{
    /// <summary>
    /// 事件发布者
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        private readonly ILogService _logService;
        private readonly IServiceResolver _serviceResolver;

        public EventPublisher(ILogService logService
            , IServiceResolver serviceResolver)
        {
            _logService = logService;
            _serviceResolver = serviceResolver;
        }

        #region Methods

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="TEvent">事件类</typeparam>
        /// <param name="e">事件对象</param>
        public virtual void Publish<TEvent>(TEvent e)
        {
            //获取所有事件接收者
            var consumers = _serviceResolver.GetAll<IConsumer<TEvent>>().ToList();
            foreach (var consumer in consumers)
            {
                try
                {
                    //处理事件
                    consumer.HandleEvent(e);
                }
                catch (Exception exception)
                {
                    _logService.Error(exception);
                }
            }
        }

        #endregion Methods
    }
}