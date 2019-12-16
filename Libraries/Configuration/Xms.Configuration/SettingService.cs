using System.Collections.Generic;
using System.Linq;
using Xms.Caching;
using Xms.Configuration.Data;
using Xms.Configuration.Domain;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Identity;
using Xms.Infrastructure.Utility;

namespace Xms.Configuration
{
    /// <summary>
    /// 参数配置服务
    /// </summary>
    public class SettingService : ISettingService
    {
        private readonly ISettingRepository _settingRepository;
        private readonly CacheManager<Setting> _cache;
        private readonly IAppContext _appContext;
        private readonly ICurrentUser _currentUser;

        public SettingService(IAppContext appContext
            , ISettingRepository settingRepository)
        {
            _appContext = appContext;
            _currentUser = _appContext.GetFeature<ICurrentUser>();
            _settingRepository = settingRepository;
            _cache = new CacheManager<Setting>($"{_appContext.OrganizationUniqueName}:settings", SettingCache.BuildKey,true);
        }

        public virtual bool SaveMany(IList<Setting> entities)
        {
            if (entities.IsEmpty())
            {
                return false;
            }
            var names = entities.Select(f => f.Name);
            var result = true;
            using (UnitOfWork.Build(_settingRepository.DbContext))
            {
                _settingRepository.DeleteMany(x => x.Name.In(names));
                foreach (var item in entities)
                {
                    item.CreatedBy = _currentUser.SystemUserId;
                    item.OrganizationId = _currentUser.OrganizationId;
                }
                result = _settingRepository.CreateMany(entities);
                //add to cache                
                foreach (var deleted in entities)
                {
                    //remove from cache
                    _cache.RemoveEntity(deleted);
                }
            }
            return result;
        }

        public virtual bool Save<T>(T setting, string nameSpace = "")
        {
            var type = typeof(T);
            nameSpace = nameSpace.IfEmpty(type.FullName);
            List<Setting> entities = new List<Setting>();
            foreach (var prop in type.GetProperties())
            {
                if (!prop.CanRead)
                {
                    continue;
                }

                var key = nameSpace + "." + prop.Name;
                var value = prop.GetValue(setting);
                if (value == null)
                {
                    continue;
                }
                if (!prop.PropertyType.IsValueType())
                {
                    value = value.SerializeToJson();
                }
                var entity = new Setting
                {
                    Name = key,
                    Value = value.ToString()
                };
                entities.Add(entity);
            }
            return SaveMany(entities);
        }

        public virtual bool Save(IDictionary<string, object> keyValues)
        {
            if (keyValues.IsEmpty())
            {
                return false;
            }
            List<Setting> entities = new List<Setting>();
            foreach (var item in keyValues)
            {
                if (item.Value == null)
                {
                    continue;
                }
                var value = item.Value;
                if (!value.GetType().IsValueType())
                {
                    value = value.SerializeToJson();
                }
                var entity = new Setting
                {
                    Name = item.Key,
                    Value = value.ToString()
                };
                entities.Add(entity);
            }
            return SaveMany(entities);
        }
    }
}