using Xms.Context;
using Xms.DataMapping.Domain;

namespace Xms.DataMapping
{
    /// <summary>
    /// 单据转换缓存
    /// </summary>
    public class EntityMapCache
    {
        public static string CacheKey(IAppContext appContext)
        {
            return appContext.OrganizationUniqueName + ":entitymaps";
        }

        public static string BuildKey(EntityMap entity)
        {
            return entity.SourceEntityId + "/" + entity.TargetEntityId + "/" + entity.EntityMapId;
        }
    }
}