namespace Xms.Event.Abstractions
{
    /// <summary>
    /// 事件发布接口
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="e"></param>
        void Publish<TEvent>(TEvent e);
    }
}