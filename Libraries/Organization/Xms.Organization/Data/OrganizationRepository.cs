using Xms.Core.Data;
using Xms.Data;

namespace Xms.Organization.Data
{
    /// <summary>
    /// 组织仓储
    /// </summary>
    public class OrganizationRepository : DefaultRepository<Domain.Organization>, IOrganizationRepository
    {
        public OrganizationRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}