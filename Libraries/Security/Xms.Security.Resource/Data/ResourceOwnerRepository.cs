using Xms.Core.Data;
using Xms.Data;

namespace Xms.Security.Resource.Data
{
    /// <summary>
    /// 权限资源类型仓储
    /// </summary>
    public class ResourceOwnerRepository : DefaultRepository<Domain.ResourceOwner>, IResourceOwnerRepository
    {
        public ResourceOwnerRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}