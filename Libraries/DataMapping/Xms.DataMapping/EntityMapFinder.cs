using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.DataMapping.Abstractions;
using Xms.DataMapping.Data;
using Xms.DataMapping.Domain;
using Xms.Dependency.Abstractions;
using Xms.Module.Core;

namespace Xms.DataMapping
{
    /// <summary>
    /// 实体映射查找服务
    /// </summary>
    public class EntityMapFinder : IEntityMapFinder, IDependentLookup<EntityMap>
    {
        private readonly IEntityMapRepository _entityMapRepository;
        private readonly Caching.CacheManager<EntityMap> _cacheService;
        private readonly IAppContext _appContext;

        public EntityMapFinder(IAppContext appContext
            , IEntityMapRepository entityMapRepository)
        {
            _appContext = appContext;
            _entityMapRepository = entityMapRepository;
            _cacheService = new Caching.CacheManager<EntityMap>(EntityMapCache.CacheKey(_appContext), _appContext.PlatformSettings.CacheEnabled);
        }

        public EntityMap FindById(Guid id)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("EntityMapId", id.ToString());
            EntityMap entity = _cacheService.Get(dic, () =>
             {
                 return _entityMapRepository.FindById(id);
             });
            return entity;
        }

        public EntityMap FindByParentId(Guid parentid)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("ParentEntityMapId", parentid.ToString());
            EntityMap entity = _cacheService.Get(dic, () =>
             {
                 return _entityMapRepository.Find(n => n.ParentEntityMapId == parentid);
             });
            return entity;
        }

        public EntityMap Find(Guid sourceEntityId, Guid targetEntityId)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("sourceEntityId", sourceEntityId.ToString());
            dic.Add("targetEntityId", targetEntityId.ToString());
            EntityMap entity = _cacheService.Get(dic, () =>
             {
                 return _entityMapRepository.Find(n => n.SourceEntityId == sourceEntityId && n.TargetEntityId == targetEntityId);
             });
            return entity;
        }

        public PagedList<EntityMap> QueryPaged(Func<QueryDescriptor<EntityMap>, QueryDescriptor<EntityMap>> container)
        {
            QueryDescriptor<EntityMap> q = container(QueryDescriptorBuilder.Build<EntityMap>());
            var datas = _entityMapRepository.QueryPaged(q);
            return datas;
        }

        public PagedList<EntityMap> QueryPaged(Func<QueryDescriptor<EntityMap>, QueryDescriptor<EntityMap>> container, Guid solutionId, bool existInSolution)
        {
            QueryDescriptor<EntityMap> q = container(QueryDescriptorBuilder.Build<EntityMap>());
            var datas = _entityMapRepository.QueryPaged(q, Module.Core.ModuleCollection.GetIdentity(DataMappingDefaults.ModuleName), solutionId, existInSolution);

            return datas;
        }

        public List<EntityMap> Query(Func<QueryDescriptor<EntityMap>, QueryDescriptor<EntityMap>> container)
        {
            QueryDescriptor<EntityMap> q = container(QueryDescriptorBuilder.Build<EntityMap>());
            var datas = _entityMapRepository.Query(q)?.ToList();
            return datas;
        }

        public List<EntityMap> FindAll()
        {
            var entities = _cacheService.GetVersionItems("all", () =>
             {
                 return PreCacheAll();
             });
            return entities;
        }

        private List<EntityMap> PreCacheAll()
        {
            return _entityMapRepository.FindAll()?.ToList();
        }

        #region dependency

        public DependentDescriptor GetDependent(Guid dependentId)
        {
            var result = FindById(dependentId);
            return result != null ? new DependentDescriptor() { Name = result.SourceEnttiyName } : null;
        }

        public int ComponentType => ModuleCollection.GetIdentity(DataMappingDefaults.ModuleName);

        #endregion dependency
    }
}