using System.Threading.Tasks;
using Xms.Flow.Core;

namespace Xms.Flow
{
    public interface IWorkFlowStarter
    {
        WorkFlowExecutionResult OnStarted(WorkFlowStartUpContext context, WorkFlowExecutionResult result);

        WorkFlowExecutionResult OnStarting(WorkFlowStartUpContext context, WorkFlowExecutionResult result);

        Task<WorkFlowExecutionResult> StartAsync(WorkFlowStartUpContext context);
    }
}