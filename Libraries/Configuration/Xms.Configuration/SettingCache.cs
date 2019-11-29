using Xms.Configuration.Domain;

namespace Xms.Configuration
{
    /// <summary>
    /// 配置信息缓存服务
    /// </summary>
    public class SettingCache
    {
        public static string BuildKey(Setting entity)
        {
            return entity.OrganizationId + "/" + entity.Name + "/";
        }
    }
}