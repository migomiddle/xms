using Xms.Core.Data;
using Xms.Data;
using Xms.Security.Domain;

namespace Xms.Security.DataAuthorization.Data
{
    /// <summary>
    /// 实体权限项仓储
    /// </summary>
    public class EntityPermissionRepository : DefaultRepository<EntityPermission>, IEntityPermissionRepository
    {
        public EntityPermissionRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}