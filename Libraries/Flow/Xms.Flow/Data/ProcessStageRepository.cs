using Xms.Core.Data;
using Xms.Data;
using Xms.Flow.Domain;

namespace Xms.Flow.Data
{
    /// <summary>
    /// 流程执行阶段仓储
    /// </summary>
    public class ProcessStageRepository : DefaultRepository<ProcessStage>, IProcessStageRepository
    {
        public ProcessStageRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}