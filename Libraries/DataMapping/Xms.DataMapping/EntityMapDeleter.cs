using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.DataMapping.Data;
using Xms.DataMapping.Domain;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;

namespace Xms.DataMapping
{
    /// <summary>
    /// 实体映射删除服务
    /// </summary>
    public class EntityMapDeleter : IEntityMapDeleter, ICascadeDelete<Schema.Domain.Entity>
    {
        private readonly IEntityMapRepository _entityMapRepository;
        private readonly Caching.CacheManager<EntityMap> _cacheService;
        private readonly IEntityMapDependency _dependencyService;
        private readonly IEnumerable<ICascadeDelete<EntityMap>> _cascadeDeletes;
        private readonly IAppContext _appContext;

        public EntityMapDeleter(IAppContext appContext
            , IEntityMapRepository entityMapRepository
            , IEntityMapDependency dependencyService
            , IEnumerable<ICascadeDelete<EntityMap>> cascadeDeletes
            )
        {
            _appContext = appContext;
            _entityMapRepository = entityMapRepository;
            _dependencyService = dependencyService;
            _cascadeDeletes = cascadeDeletes;
            _cacheService = new Caching.CacheManager<EntityMap>(EntityMapCache.CacheKey(_appContext), _appContext.PlatformSettings.CacheEnabled);
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的实体</param>
        public void CascadeDelete(params Schema.Domain.Entity[] parent)
        {
            if (parent.IsEmpty())
            {
                return;
            }
            var entityIds = parent.Select(x => x.EntityId).ToArray();
            var deleteds = _entityMapRepository.Query(x => x.SourceEntityId.In(entityIds));
            if (deleteds.NotEmpty())
            {
                DeleteCore(deleteds.ToArray());
            }
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = true;
            var deleteds = _entityMapRepository.Query(x => x.EntityMapId.In(id));
            if (deleteds.NotEmpty())
            {
                result = DeleteCore(deleteds.ToArray());
            }
            return result;
        }

        private bool DeleteCore(params EntityMap[] deleteds)
        {
            Guard.NotEmpty(deleteds, nameof(deleteds));
            var ids = deleteds.Select(x => x.EntityMapId);
            var result = true;
            using (UnitOfWork.Build(_entityMapRepository.DbContext))
            {
                result = _entityMapRepository.DeleteMany(ids);
                //删除依赖项
                _dependencyService.Delete(ids.ToArray());
                foreach (var deleted in deleteds)
                {
                    //cascade delete
                    _cascadeDeletes?.ToList().ForEach((x) => { x.CascadeDelete(deleted); });
                    //remove from cache
                    _cacheService.RemoveEntity(deleted);
                }
                //删除子项
                var childs = _entityMapRepository.Query(x => x.ParentEntityMapId.In(ids));
                if (childs.NotEmpty())
                {
                    DeleteCore(childs.ToArray());
                }
            }
            return result;
        }
    }
}