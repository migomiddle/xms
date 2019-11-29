namespace Xms.Flow.Core.Events
{
    /// <summary>
    /// 工作流执行后事件
    /// </summary>
    public class WorkFlowExecutedEvent
    {
        public WorkFlowExecutionContext Context { get; set; }
        public WorkFlowExecutionResult Result { get; set; }
    }
}