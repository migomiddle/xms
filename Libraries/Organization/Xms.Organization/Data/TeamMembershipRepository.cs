using Xms.Core.Data;
using Xms.Data;
using Xms.Organization.Domain;

namespace Xms.Organization.Data
{
    /// <summary>
    /// 团队成员仓储
    /// </summary>
    public class TeamMembershipRepository : DefaultRepository<TeamMembership>, ITeamMembershipRepository
    {
        public TeamMembershipRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}