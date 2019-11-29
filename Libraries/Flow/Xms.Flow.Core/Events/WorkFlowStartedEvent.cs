namespace Xms.Flow.Core.Events
{
    /// <summary>
    /// 工作流启动后事件
    /// </summary>
    public class WorkFlowStartedEvent
    {
        public WorkFlowStartUpContext Context { get; set; }
        public WorkFlowExecutionResult Result { get; set; }
    }
}