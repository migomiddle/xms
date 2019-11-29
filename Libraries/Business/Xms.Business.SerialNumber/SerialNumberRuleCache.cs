using Xms.Business.SerialNumber.Domain;
using Xms.Context;

namespace Xms.Business.SerialNumber
{
    /// <summary>
    /// 自动编号规则缓存
    /// </summary>
    public class SerialNumberRuleCache
    {
        public static string CacheKey(IAppContext appContext)
        {
            return appContext.OrganizationUniqueName + ":serialnumberrules";
        }

        public static string BuildKey(SerialNumberRule entity)
        {
            return entity.EntityId + "/" + entity.SerialNumberRuleId + "/";
        }
    }
}