using Xms.Core.Data;
using Xms.Data;
using Xms.Schema.Domain;

namespace Xms.Schema.Data
{
    /// <summary>
    /// 选项集项目仓储
    /// </summary>
    public class OptionSetDetailRepository : DefaultRepository<OptionSetDetail>, IOptionSetDetailRepository
    {
        public OptionSetDetailRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}