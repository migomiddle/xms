using Xms.Core.Data;
using Xms.Data;
using Xms.Flow.Domain;

namespace Xms.Flow.Data
{
    /// <summary>
    /// 流程步骤仓储
    /// </summary>
    public class WorkFlowStepRepository : DefaultRepository<WorkFlowStep>, IWorkFlowStepRepository
    {
        public WorkFlowStepRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}