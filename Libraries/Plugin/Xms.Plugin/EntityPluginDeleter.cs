using System;
using System.Linq;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Dependency;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Plugin.Abstractions;
using Xms.Plugin.Data;
using Xms.Plugin.Domain;
using Xms.Solution;

namespace Xms.Plugin
{
    /// <summary>
    /// 实体插件删除服务
    /// </summary>
    public class EntityPluginDeleter : IEntityPluginDeleter, ICascadeDelete<Schema.Domain.Entity>
    {
        private readonly IEntityPluginRepository _entityPluginRepository;
        private readonly ISolutionComponentService _solutionComponentService;
        private readonly Caching.CacheManager<EntityPlugin> _cacheService;
        private readonly IDependencyService _dependencyService;

        public EntityPluginDeleter(IAppContext appContext
            , IEntityPluginRepository entityPluginRepository
            , ISolutionComponentService solutionComponentService
            , IDependencyService dependencyService)
        {
            _entityPluginRepository = entityPluginRepository;
            _solutionComponentService = solutionComponentService;
            _dependencyService = dependencyService;
            _cacheService = new Caching.CacheManager<EntityPlugin>(EntityPluginCache.GetCacheKey(appContext), appContext.PlatformSettings.CacheEnabled);
        }

        public bool DeleteById(params Guid[] ids)
        {
            Guard.NotEmpty(ids, nameof(ids));
            var deleteds = _entityPluginRepository.Query(x => x.EntityPluginId.In(ids.ToArray()));
            if (deleteds.IsEmpty())
            {
                return false;
            }
            return DeleteCore(deleteds.ToArray());
        }

        public bool DeleteByEntityId(Guid entityId)
        {
            var deleteds = _entityPluginRepository.Query(x => x.EntityId == entityId);
            if (deleteds.IsEmpty())
            {
                return false;
            }
            return DeleteCore(deleteds.ToArray());
        }

        private bool DeleteCore(params EntityPlugin[] deleteds)
        {
            Guard.NotEmpty(deleteds, nameof(deleteds));
            var result = true;
            var solutionId = deleteds.First().SolutionId;
            var ids = deleteds.Select(x => x.EntityPluginId).ToArray();
            using (UnitOfWork.Build(_entityPluginRepository.DbContext))
            {
                result = _entityPluginRepository.DeleteMany(ids);
                //solution component
                _solutionComponentService.DeleteObject(solutionId, PluginDefaults.ModuleName, ids);
                //删除依赖项
                _dependencyService.DeleteByDependentId(PluginDefaults.ModuleName, ids);
                //remove from cache
                foreach (var deleted in deleteds)
                {
                    _cacheService.RemoveEntity(deleted);
                }
            }
            return result;
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
            var deleteds = _entityPluginRepository.Query(x => x.EntityId.In(entityIds));
            if (deleteds.NotEmpty())
            {
                DeleteCore(deleteds.ToArray());
            }
        }
    }
}