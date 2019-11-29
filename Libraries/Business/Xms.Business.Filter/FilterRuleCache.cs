using Xms.Business.Filter.Domain;
using Xms.Context;

namespace Xms.Business.Filter
{
    /// <summary>
    /// 拦截规则缓存
    /// </summary>
    public class FilterRuleCache
    {
        public static string CacheKey(IAppContext appContext)
        {
            return appContext.OrganizationUniqueName + ":duplicaterules";
        }

        public static string BuildKey(FilterRule entity)
        {
            return entity.EntityId + "/" + entity.FilterRuleId.ToString() + "/";
        }
    }
}