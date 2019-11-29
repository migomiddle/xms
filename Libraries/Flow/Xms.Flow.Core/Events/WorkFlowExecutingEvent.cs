namespace Xms.Flow.Core.Events
{
    /// <summary>
    /// 工作流执行前事件
    /// </summary>
    public class WorkFlowExecutingEvent
    {
        public WorkFlowExecutionContext Context { get; set; }
    }
}