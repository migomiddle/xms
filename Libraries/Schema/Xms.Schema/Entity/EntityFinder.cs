using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Context;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Dependency.Abstractions;
using Xms.Localization;
using Xms.Module.Core;
using Xms.Schema.Abstractions;
using Xms.Schema.Data;

namespace Xms.Schema.Entity
{
    /// <summary>
    /// 实体查询服务
    /// </summary>
    public class EntityFinder : IEntityFinder, IDependentLookup<Domain.Entity>
    {
        private readonly IEntityRepository _entityRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly Caching.CacheManager<Domain.Entity> _cacheService;
        private readonly IAppContext _appContext;

        public EntityFinder(IAppContext appContext
            , IEntityRepository entityRepository
            , ILocalizedLabelService localizedLabelService
            )
        {
            _appContext = appContext;
            _entityRepository = entityRepository;
            _localizedLabelService = localizedLabelService;
            _cacheService = new Caching.CacheManager<Domain.Entity>(_appContext.OrganizationUniqueName + ":entities", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Exists(string name)
        {
            return _entityRepository.Exists(n => n.OrganizationId == _appContext.OrganizationId && n.Name == name);
        }

        public Domain.Entity FindById(Guid id)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("entityid", id.ToString());

            var entity = _cacheService.Get(dic, () =>
            {
                return _entityRepository.FindById(id);
            });

            if (entity != null)
            {
                WrapLocalizedLabel(entity);
            }
            return entity;
        }

        public Domain.Entity FindByName(string name)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("name", name);
            var entity = _cacheService.Get(dic, () =>
            {
                return _entityRepository.Find(n => n.Name == name);
            });

            if (entity != null)
            {
                WrapLocalizedLabel(entity);
            }
            return entity;
        }

        public List<Domain.Entity> FindByNames(params string[] name)
        {
            string sIndex = string.Join("/", name);
            var entities = _cacheService.GetVersionItems(sIndex, () =>
             {
                 return this.Query(n => n.Where(f => f.Name.In(name)));
             }
            );

            if (entities != null)
            {
                WrapLocalizedLabel(entities);
            }
            return entities;
        }

        public List<Domain.Entity> FindAll()
        {
            var entities = _cacheService.GetVersionItems("all", () =>
             {
                 return _entityRepository.FindAll()?.ToList();
             }
            );

            if (entities != null)
            {
                WrapLocalizedLabel(entities);
            }
            return entities;
        }

        public PagedList<Domain.Entity> QueryPaged(Func<QueryDescriptor<Domain.Entity>, QueryDescriptor<Domain.Entity>> container)
        {
            QueryDescriptor<Domain.Entity> q = container(QueryDescriptorBuilder.Build<Domain.Entity>());

            var datas = _entityRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public PagedList<Domain.Entity> QueryPaged(Func<QueryDescriptor<Domain.Entity>, QueryDescriptor<Domain.Entity>> container, Guid solutionId, bool existInSolution)
        {
            QueryDescriptor<Domain.Entity> q = container(QueryDescriptorBuilder.Build<Domain.Entity>());
            var datas = _entityRepository.QueryPaged(q, ModuleCollection.GetIdentity(EntityDefaults.ModuleName), solutionId, existInSolution);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<Domain.Entity> Query(Func<QueryDescriptor<Domain.Entity>, QueryDescriptor<Domain.Entity>> container)
        {
            QueryDescriptor<Domain.Entity> q = container(QueryDescriptorBuilder.Build<Domain.Entity>());
            var datas = _entityRepository.Query(q).ToList();

            WrapLocalizedLabel(datas);
            return datas;
        }

        public List<Domain.Entity> QueryRelated(Guid entityid, RelationShipType type, int cascadeLinkMask = -1)
        {
            var datas = _entityRepository.QueryRelated(entityid, type).ToList();
            WrapLocalizedLabel(datas);
            return datas;
        }

        private List<Domain.Entity> PreCacheAll()
        {
            return _entityRepository.FindAll()?.ToList();
        }

        #region dependency

        public int ComponentType => ModuleCollection.GetIdentity(EntityDefaults.ModuleName);

        public DependentDescriptor GetDependent(Guid dependentId)
        {
            var result = FindById(dependentId);
            return result != null ? new DependentDescriptor() { Name = result.LocalizedName } : null;
        }

        #endregion dependency

        #region 多语言标签

        private void WrapLocalizedLabel(IEnumerable<Domain.Entity> entities)
        {
            /*
            if (entities.NotEmpty())
            {
                var ids = entities.Select(f => f.EntityId);
                if (ids.Count() > 1000)
                {
                    int count = ids.Count();
                    while (count > 1000)
                    {
                        var tmpIds = ids.Take(1000);
                        var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId.In(tmpIds)));
                        if (labels.NotEmpty())
                        {
                            foreach (var d in entities)
                            {
                                d.LocalizedName = _localizedLabelService.GetLabelText(labels, d.EntityId, "LocalizedName", d.LocalizedName);
                                d.Description = _localizedLabelService.GetLabelText(labels, d.EntityId, "Description", d.Description);
                            }
                        }
                        count -= 1000;
                    }
                }
                else
                {
                    var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId.In(ids)));
                    if (labels.NotEmpty())
                    {
                        foreach (var d in entities)
                        {
                            d.LocalizedName = _localizedLabelService.GetLabelText(labels, d.EntityId, "LocalizedName", d.LocalizedName);
                            d.Description = _localizedLabelService.GetLabelText(labels, d.EntityId, "Description", d.Description);
                        }
                    }
                }
            }*/
        }

        private void WrapLocalizedLabel(Domain.Entity entity)
        {
            /*
            var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId == entity.EntityId));
            entity.LocalizedName = _localizedLabelService.GetLabelText(labels, entity.EntityId, "LocalizedName", entity.LocalizedName);
            entity.Description = _localizedLabelService.GetLabelText(labels, entity.EntityId, "Description", entity.Description);
            */
        }

        #endregion 多语言标签
    }
}