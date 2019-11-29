using Xms.Core.Data;
using Xms.Data;
using Xms.Security.Domain;

namespace Xms.Security.DataAuthorization.Data
{
    /// <summary>
    /// 角色授权数据仓储
    /// </summary>
    public class RoleObjectAccessRepository : DefaultRepository<RoleObjectAccess>, IRoleObjectAccessRepository
    {
        public RoleObjectAccessRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}