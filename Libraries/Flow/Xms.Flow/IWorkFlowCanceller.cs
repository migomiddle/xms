using Xms.Flow.Core;

namespace Xms.Flow
{
    public interface IWorkFlowCanceller
    {
        WorkFlowCancellationResult Cancel(WorkFlowCancellationContext context);

        WorkFlowCancellationResult OnCancelled(WorkFlowCancellationContext context, WorkFlowCancellationResult result);

        WorkFlowCancellationResult OnCancelling(WorkFlowCancellationContext context, WorkFlowCancellationResult result);
    }
}