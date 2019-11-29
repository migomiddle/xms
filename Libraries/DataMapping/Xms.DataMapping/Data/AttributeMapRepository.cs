using Xms.Core.Data;
using Xms.Data;
using Xms.DataMapping.Domain;

namespace Xms.DataMapping.Data
{
    /// <summary>
    /// 实体转换字段映射仓储
    /// </summary>
    public class AttributeMapRepository : DefaultRepository<AttributeMap>, IAttributeMapRepository
    {
        public AttributeMapRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}