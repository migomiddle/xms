using Xms.Core.Data;
using Xms.Data;
using Xms.Security.Domain;

namespace Xms.Security.DataAuthorization.Data
{
    /// <summary>
    /// 数据授权仓储
    /// </summary>
    public class PrincipalObjectAccessRepository : DefaultRepository<PrincipalObjectAccess>, IPrincipalObjectAccessRepository
    {
        public PrincipalObjectAccessRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}