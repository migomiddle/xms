using Xms.Core.Data;
using Xms.Data;
using Xms.Solution.Domain;

namespace Xms.Solution.Data
{
    /// <summary>
    /// 解决方案组件仓储
    /// </summary>
    public class SolutionComponentRepository : DefaultRepository<SolutionComponent>, ISolutionComponentRepository
    {
        public SolutionComponentRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}