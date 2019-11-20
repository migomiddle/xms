using Xms.Core.Data;
using Xms.Data;

namespace Xms.Schema.Data
{
    /// <summary>
    /// 字段选项仓储
    /// </summary>
    public class StringMapRepository : DefaultRepository<Domain.StringMap>, IStringMapRepository
    {
        public StringMapRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}