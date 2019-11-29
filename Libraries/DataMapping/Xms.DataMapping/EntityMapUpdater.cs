using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Core;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Data.Provider;
using Xms.DataMapping.Data;
using Xms.DataMapping.Domain;

namespace Xms.DataMapping
{
    /// <summary>
    /// 实体映射更新服务
    /// </summary>
    public class EntityMapUpdater : IEntityMapUpdater
    {
        private readonly IEntityMapRepository _entityMapRepository;
        private readonly Caching.CacheManager<EntityMap> _cacheService;
        private readonly IAppContext _appContext;

        public EntityMapUpdater(IAppContext appContext
            , IEntityMapRepository entityMapRepository)
        {
            _appContext = appContext;
            _entityMapRepository = entityMapRepository;
            _cacheService = new Caching.CacheManager<EntityMap>(EntityMapCache.CacheKey(_appContext), _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Update(EntityMap entity)
        {
            bool result = true;
            using (UnitOfWork.Build(_entityMapRepository.DbContext))
            {
                result = _entityMapRepository.Update(entity);
                ////内容不全，不能缓存
                _cacheService.RemoveEntity(entity);
            }
            return result;
        }

        public bool UpdateState(IEnumerable<Guid> ids, bool isEnabled)
        {
            var context = UpdateContextBuilder.Build<EntityMap>();
            context.Set(f => f.StateCode, isEnabled ? RecordState.Enabled : RecordState.Disabled);
            context.Where(f => f.EntityMapId.In(ids));
            var result = true;
            using (UnitOfWork.Build(_entityMapRepository.DbContext))
            {
                result = _entityMapRepository.Update(context);
                //set to cache
                var items = _entityMapRepository.Query(x => x.EntityMapId.In(ids));
                foreach (var item in items)
                {
                    _cacheService.SetListItem(item);
                }
            }
            return result;
        }
    }
}