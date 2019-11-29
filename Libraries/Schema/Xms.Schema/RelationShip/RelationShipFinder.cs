using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Attribute;
using Xms.Schema.Data;
using Xms.Schema.Entity;

namespace Xms.Schema.RelationShip
{
    /// <summary>
    /// 关系查询服务
    /// </summary>
    public class RelationShipFinder : IRelationShipFinder
    {
        private readonly IRelationShipRepository _relationShipRepository;
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly Caching.CacheManager<Domain.RelationShip> _cacheService;
        private readonly IAppContext _appContext;

        public RelationShipFinder(IAppContext appContext
            , IRelationShipRepository relationShipRepository
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder)
        {
            _appContext = appContext;
            _relationShipRepository = relationShipRepository;
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _cacheService = new Caching.CacheManager<Domain.RelationShip>(_appContext.OrganizationUniqueName + ":relationships", _appContext.PlatformSettings.CacheEnabled);
        }

        public Domain.RelationShip FindById(Guid id)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("RelationshipId", id.ToString());

            Domain.RelationShip entity = _cacheService.Get(dic, () =>
             {
                 return _relationShipRepository.FindById(id);
             });
            return entity;
        }

        public Domain.RelationShip FindByName(string name)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("name", name);

            Domain.RelationShip entity = _cacheService.Get(dic, () =>
             {
                 return _relationShipRepository.Find(x => x.Name == name);
             });
            return entity;
        }

        public Domain.RelationShip Find(Expression<Func<Domain.RelationShip, bool>> predicate)
        {
            var data = _relationShipRepository.Find(predicate);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public PagedList<Domain.RelationShip> QueryPaged(Func<QueryDescriptor<Domain.RelationShip>, QueryDescriptor<Domain.RelationShip>> container)
        {
            QueryDescriptor<Domain.RelationShip> q = container(QueryDescriptorBuilder.Build<Domain.RelationShip>());
            var datas = _relationShipRepository.QueryPaged(q);

            //WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<Domain.RelationShip> Query(Func<QueryDescriptor<Domain.RelationShip>, QueryDescriptor<Domain.RelationShip>> container)
        {
            QueryDescriptor<Domain.RelationShip> q = container(QueryDescriptorBuilder.Build<Domain.RelationShip>());
            var datas = _relationShipRepository.Query(q)?.ToList();

            //WrapLocalizedLabel(datas);
            return datas;
        }

        public List<Domain.RelationShip> QueryByEntityId(Guid? referencingEntityId, Guid? referencedEntityId)
        {
            List<Domain.RelationShip> result = _cacheService.GetVersionItems(referencingEntityId + "/" + referencedEntityId, () =>
             {
                 if (referencingEntityId.HasValue && referencedEntityId.HasValue)
                 {
                     return _relationShipRepository.Query(x => x.ReferencingEntityId == referencingEntityId.Value && x.ReferencedEntityId == referencedEntityId.Value)?.ToList();
                 }
                 else if (referencingEntityId.HasValue)
                 {
                     return _relationShipRepository.Query(x => x.ReferencingEntityId == referencingEntityId.Value)?.ToList();
                 }
                 return _relationShipRepository.Query(x => x.ReferencedEntityId == referencedEntityId.Value)?.ToList();
             });
            //WrapLocalizedLabel(result);
            return result;
        }

        public List<Domain.RelationShip> FindAll()
        {
            var entities = _cacheService.GetVersionItems("all", () =>
             {
                 return PreCacheAll();
             });
            if (entities != null)
            {
                WrapLocalizedLabel(entities);
            }
            return entities;
        }

        private List<Domain.RelationShip> PreCacheAll()
        {
            return _relationShipRepository.FindAll()?.ToList();
        }

        public void WrapLocalizedLabel(IList<Domain.RelationShip> datas)
        {
            if (datas.NotEmpty())
            {
                foreach (var d in datas)
                {
                    WrapLocalizedLabel(d);
                }
            }
        }

        public void WrapLocalizedLabel(Domain.RelationShip entity)
        {
            var rcingEntity = _entityFinder.FindById(entity.ReferencingEntityId);
            if (rcingEntity != null)
            {
                entity.ReferencingEntityLocalizedName = rcingEntity.LocalizedName;
            }
            var rcingAttr = _attributeFinder.FindById(entity.ReferencingAttributeId);
            if (rcingAttr != null)
            {
                entity.ReferencingAttributeLocalizedName = rcingAttr.LocalizedName;
            }
            var rcedEntity = _entityFinder.FindById(entity.ReferencedEntityId);
            if (rcedEntity != null)
            {
                entity.ReferencedEntityLocalizedName = rcedEntity.LocalizedName;
            }
            var rcedAttr = _attributeFinder.FindById(entity.ReferencedAttributeId);
            if (rcedAttr != null)
            {
                entity.ReferencedAttributeLocalizedName = rcedAttr.LocalizedName;
            }
        }
    }

    //public class RelationShipDependencyFinder : IDependencyFinder<Domain.Attribute>
    //{
    //    private readonly IRelationShipRepository _relationShipRepository;

    //    public RelationShipDependencyFinder(IRelationShipRepository relationShipRepository)
    //    {
    //        _relationShipRepository = relationShipRepository;
    //    }

    //    public DependencyComponentType ComponentType => DependencyComponentType;

    //    public DependentDescriptor GetDependent(Guid dependentId)
    //    {
    //        var result = _relationShipRepository.FindById(dependentId);
    //        return result != null ? new DependentDescriptor() { Name = result.Name } : null;
    //    }
    //}
}