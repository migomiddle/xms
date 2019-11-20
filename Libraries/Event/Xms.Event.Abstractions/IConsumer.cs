namespace Xms.Event.Abstractions
{
    /// <summary>
    /// 事件接收接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConsumer<T>
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="eventMessage">事件</param>
        void HandleEvent(T eventMessage);
    }
}