using Xms.Core.Data;
using Xms.Data;
using Xms.Security.Domain;

namespace Xms.Security.Principal.Data
{
    /// <summary>
    /// 用户安全角色仓储
    /// </summary>
    public class SystemUserRolesRepository : DefaultRepository<SystemUserRoles>, ISystemUserRolesRepository
    {
        public SystemUserRolesRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}