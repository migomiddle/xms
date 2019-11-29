using System.Threading.Tasks;
using Xms.Flow.Core;

namespace Xms.Flow
{
    public interface IWorkFlowExecuter
    {
        Task<WorkFlowExecutionResult> ExecuteAsync(WorkFlowExecutionContext context);

        WorkFlowExecutionResult OnExecuted(WorkFlowExecutionContext context, WorkFlowExecutionResult result);

        WorkFlowExecutionResult OnExecuting(WorkFlowExecutionContext context, WorkFlowExecutionResult result);
    }
}