namespace Xms.Flow.Abstractions
{
    public enum WorkFlowProcessState
    {
        None = -1,
        Waiting,
        Processing,
        Passed,
        UnPassed,
        Disabled,
        Canceled
    }
}