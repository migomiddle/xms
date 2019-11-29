using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Caching;
using Xms.Configuration.Data;
using Xms.Configuration.Domain;
using Xms.Core.Data;
using Xms.Core.Org;
using Xms.Infrastructure.Utility;

namespace Xms.Configuration
{
    /// <summary>
    /// 配置信息查找服务
    /// </summary>
    public class SettingFinder : ISettingFinder
    {
        private readonly ISettingRepository _settingRepository;
        private readonly IOrgDataServer _orgDataServer;
        private readonly CacheManager<Setting> _cache;

        public SettingFinder(IOrgDataServer orgDataServer
            , ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
            _orgDataServer = orgDataServer;
            _cache = new CacheManager<Setting>($"{_orgDataServer.UniqueName}:settings", SettingCache.BuildKey, true);
        }

        public virtual T Get<T>(string nameSpace = "") where T : new()
        {
            var type = typeof(T);
            nameSpace = nameSpace.IfEmpty(type.FullName);
            var settings = _cache.GetItemsByPattern(() =>
            {
                return _settingRepository.Query(x => x.OrganizationId == _orgDataServer.OrganizationBaseId && x.Name.Like($"{nameSpace}.%"))?.ToList();
            }, _orgDataServer.OrganizationBaseId + "/" + nameSpace + "*/");
            var result = new T();
            if (settings.NotEmpty())
            {
                foreach (var prop in type.GetProperties())
                {
                    if (!prop.CanRead || !prop.CanWrite)
                    {
                        continue;
                    }

                    var key = nameSpace + "." + prop.Name;
                    var setting = settings.FirstOrDefault(x => x.Name.IsCaseInsensitiveEqual(key));
                    if (setting == null || setting.Value.IsEmpty())
                    {
                        continue;
                    }
                    object value = setting.Value;

                    if (!prop.PropertyType.IsValueType())
                    {
                        value = prop.PropertyType.DeserializeFromJson(value.ToString());
                    }
                    else
                    {
                        if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                        {
                            continue;
                        }

                        //if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(value))//enum is invalid
                        //{
                        //    continue;
                        //}
                        value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting.Value);
                    }
                    prop.SetValue(result, value, null);
                }
            }
            return result;
        }

        public virtual Dictionary<string, string> GetKeyValues(string nameSpace = "")
        {
            List<Setting> settings;
            if (nameSpace.IsNotEmpty())
            {
                settings = _settingRepository.Query(x => x.OrganizationId == _orgDataServer.OrganizationBaseId && x.Name.Like($"{nameSpace}.%"))?.ToList();
            }
            else
            {
                settings = _settingRepository.Query(x => x.OrganizationId == _orgDataServer.OrganizationBaseId)?.ToList();
            }
            if (settings.NotEmpty())
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                foreach (var item in settings)
                {
                    result.Add(item.Name, item.Value);
                }
                return result;
            }
            return null;
        }
    }
}