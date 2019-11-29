using Xms.Core.Data;
using Xms.Data;
using Xms.Flow.Domain;

namespace Xms.Flow.Data
{
    /// <summary>
    /// 流程实例仓储
    /// </summary>
    public class WorkFlowInstanceRepository : DefaultRepository<WorkFlowInstance>, IWorkFlowInstanceRepository
    {
        public WorkFlowInstanceRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}