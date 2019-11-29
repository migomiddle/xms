using Xms.Business.DuplicateValidator.Domain;
using Xms.Context;

namespace Xms.Business.DuplicateValidator
{
    /// <summary>
    /// 重复检测缓存服务
    /// </summary>
    public class DuplicateRuleCache
    {
        public static string CacheKey(IAppContext appContext)
        {
            return appContext.OrganizationUniqueName + ":duplicaterules";
        }

        public static string BuildKey(DuplicateRule entity)
        {
            return entity.EntityId + "/" + entity.DuplicateRuleId.ToString() + "/";
        }
    }
}