namespace Xms.Flow.Core.Events
{
    /// <summary>
    /// 工作流启动前事件
    /// </summary>
    public class WorkFlowStartingEvent
    {
        public WorkFlowStartUpContext Context { get; set; }
        public WorkFlowExecutionResult Result { get; set; }
    }
}