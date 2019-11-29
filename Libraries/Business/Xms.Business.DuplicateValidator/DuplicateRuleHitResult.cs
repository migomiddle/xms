using Xms.Business.DuplicateValidator.Domain;
using Xms.Core.Data;

namespace Xms.Business.DuplicateValidator
{
    /// <summary>
    /// 重复记录命中结果
    /// </summary>
    public class DuplicateRuleHitResult
    {
        public DuplicateRule Rule { get; set; }
        public Entity Target { get; set; }
    }
}