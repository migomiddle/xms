using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xms.Context;
using Xms.Core;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Data.Provider;
using Xms.Plugin.Data;
using Xms.Plugin.Domain;

namespace Xms.Plugin
{
    /// <summary>
    /// 实体插件更新服务
    /// </summary>
    public class EntityPluginUpdater : IEntityPluginUpdater
    {
        private readonly IEntityPluginRepository _entityPluginRepository;
        private readonly IEntityPluginFileProvider _entityPluginFileProvider;
        private readonly Caching.CacheManager<EntityPlugin> _cacheService;
        private readonly IAppContext _appContext;

        public EntityPluginUpdater(IAppContext appContext
            , IEntityPluginRepository entityPluginRepository
            , IEntityPluginFileProvider entityPluginFileProvider)
        {
            _appContext = appContext;
            _entityPluginRepository = entityPluginRepository;
            _entityPluginFileProvider = entityPluginFileProvider;
            _cacheService = new Caching.CacheManager<EntityPlugin>(EntityPluginCache.GetCacheKey(appContext), _appContext.PlatformSettings.CacheEnabled);
        }

        public async Task<bool> Update(EntityPlugin entity, IFormFile file)
        {
            if (file != null)
            {
                await _entityPluginFileProvider.Save(file).ConfigureAwait(false);
            }
            return Update(entity);
        }

        public bool Update(EntityPlugin entity)
        {
            var result = false;
            using (UnitOfWork.Build(_entityPluginRepository.DbContext))
            {
                result = _entityPluginRepository.Update(entity);
                //set to cache
                _cacheService.SetEntity(entity);
            }
            return result;
        }

        public bool UpdateState(IEnumerable<Guid> ids, bool isEnabled)
        {
            var context = UpdateContextBuilder.Build<EntityPlugin>();
            context.Set(f => f.StateCode, isEnabled ? RecordState.Enabled : RecordState.Disabled);
            context.Where(f => f.EntityPluginId.In(ids));
            var result = false;
            using (UnitOfWork.Build(_entityPluginRepository.DbContext))
            {
                result = _entityPluginRepository.Update(context);
                //set to cache
                var items = _entityPluginRepository.Query(x => x.EntityPluginId.In(ids));
                foreach (var item in items)
                {
                    _cacheService.SetEntity(item);
                }
            }
            return result;
        }
    }
}