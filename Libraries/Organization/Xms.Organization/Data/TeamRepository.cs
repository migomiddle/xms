using Xms.Core.Data;
using Xms.Data;
using Xms.Organization.Domain;

namespace Xms.Organization.Data
{
    /// <summary>
    /// 团队仓储
    /// </summary>
    public class TeamRepository : DefaultRepository<Team>, ITeamRepository
    {
        public TeamRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}