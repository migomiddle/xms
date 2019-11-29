using Xms.Business.DuplicateValidator.Domain;
using Xms.Core.Data;
using Xms.Data;

namespace Xms.Business.DuplicateValidator.Data
{
    /// <summary>
    /// 重复检测规则仓储
    /// </summary>
    public class DuplicateRuleConditionRepository : DefaultRepository<DuplicateRuleCondition>, IDuplicateRuleConditionRepository
    {
        public DuplicateRuleConditionRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}