using Xms.Core.Data;
using Xms.Data;
using Xms.Flow.Domain;

namespace Xms.Flow.Data
{
    /// <summary>
    /// 业务流程实例仓储
    /// </summary>
    public class BusinessProcessFlowInstanceRepository : DefaultRepository<BusinessProcessFlowInstance>, IBusinessProcessFlowInstanceRepository
    {
        public BusinessProcessFlowInstanceRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}