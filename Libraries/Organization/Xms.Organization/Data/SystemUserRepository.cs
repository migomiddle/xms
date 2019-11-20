using Xms.Core.Data;
using Xms.Data;
using Xms.Organization.Domain;

namespace Xms.Organization.Data
{
    /// <summary>
    /// 系统用户仓储
    /// </summary>
    public class SystemUserRepository : DefaultRepository<SystemUser>, ISystemUserRepository
    {
        public SystemUserRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}