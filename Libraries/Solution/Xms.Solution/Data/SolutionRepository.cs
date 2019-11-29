using Xms.Core.Data;
using Xms.Data;

namespace Xms.Solution.Data
{
    /// <summary>
    /// 解决方案数据存储
    /// </summary>
    public class SolutionRepository : DefaultRepository<Domain.Solution>, ISolutionRepository
    {
        public SolutionRepository(IDbContext dbContext
            ) : base(dbContext)
        {
        }
    }
}