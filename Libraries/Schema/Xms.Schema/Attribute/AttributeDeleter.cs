using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Dependency;
using Xms.Event.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Localization.Abstractions;
using Xms.Schema.Abstractions;
using Xms.Schema.Data;
using Xms.Schema.Entity;

namespace Xms.Schema.Attribute
{
    /// <summary>
    /// 字段删除服务
    /// </summary>
    public class AttributeDeleter : IAttributeDeleter, ICascadeDelete<Domain.Entity>
    {
        private readonly IAttributeRepository _attributeRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly ILocalizedTextProvider _loc;
        private readonly IEntityFinder _entityFinder;
        private readonly IMetadataService _metadataService;
        private readonly IAttributeDependency _dependencyService;
        private readonly IDependencyChecker _dependencyChecker;
        private readonly IEventPublisher _eventPublisher;
        private readonly IEnumerable<ICascadeDelete<Domain.Attribute>> _cascadeDeletes;
        private readonly Caching.CacheManager<Domain.Attribute> _cacheService;
        private readonly IAppContext _appContext;

        public AttributeDeleter(IAppContext appContext
            , IAttributeRepository attributeRepository
            , IEntityFinder entityFinder
            , ILocalizedLabelService localizedLabelService
            , IMetadataService metadataService
            , IAttributeDependency dependencyService
            , IDependencyChecker dependencyChecker
            , IEventPublisher eventPublisher
            , IEnumerable<ICascadeDelete<Domain.Attribute>> cascadeDeletes)
        {
            _appContext = appContext;
            _attributeRepository = attributeRepository;
            _loc = _appContext.GetFeature<ILocalizedTextProvider>();
            _localizedLabelService = localizedLabelService;
            _entityFinder = entityFinder;
            _metadataService = metadataService;
            _dependencyService = dependencyService;
            _dependencyChecker = dependencyChecker;
            _eventPublisher = eventPublisher;
            _cascadeDeletes = cascadeDeletes;
            _cacheService = new Caching.CacheManager<Domain.Attribute>(_appContext.OrganizationUniqueName + ":attributes", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = false;
            var deleteds = _attributeRepository.Query(x => x.AttributeId.In(id));
            if (deleteds.NotEmpty())
            {
                result = DeleteCore(deleteds, (deleted) =>
                {
                    //检查是否允许删除
                    if (deleted == null || !deleted.IsCustomField)
                    {
                        throw new XmsException(_loc["attribute_notallow_delete"]);
                    }
                    //检查是否存在引用
                    _dependencyChecker.CheckAndThrow<Domain.Attribute>(AttributeDefaults.ModuleName, deleted.AttributeId);
                    return true;
                });
            }
            return result;
        }

        /// <summary>
        /// 实体级联删除
        /// </summary>
        /// <param name="parent">被删除的实体</param>
        public void CascadeDelete(params Domain.Entity[] parent)
        {
            if (parent.IsEmpty())
            {
                return;
            }
            var entityIds = parent.Select(x => x.EntityId).ToArray();
            var attributes = _attributeRepository.Query(x => x.EntityId.In(entityIds));
            if (attributes.NotEmpty())
            {
                var ids = attributes.Select(x => x.AttributeId).ToArray();
                using (UnitOfWork.Build(_attributeRepository.DbContext))
                {
                    //cascade delete
                    _cascadeDeletes?.ToList().ForEach((x) => { x.CascadeDelete(attributes.ToArray()); });
                    _attributeRepository.DeleteMany(x => x.EntityId.In(entityIds));
                    //删除依赖项
                    _dependencyService.Delete(ids);
                    //localization
                    _localizedLabelService.DeleteByObject(ids);
                    foreach (var deleted in attributes)
                    {
                        //remove from cache
                        _cacheService.RemoveEntity(deleted);
                    }
                }
            }
        }

        private bool DeleteCore(IEnumerable<Domain.Attribute> deleteds, Func<Domain.Attribute, bool> validation)
        {
            Guard.NotEmpty(deleteds, nameof(deleteds));
            var result = true;
            if (validation != null)
            {
                foreach (var deleted in deleteds)
                {
                    result = validation?.Invoke(deleted) ?? true;
                }
            }
            if (result)
            {
                var ids = deleteds.Select(x => x.AttributeId).ToArray();
                using (UnitOfWork.Build(_attributeRepository.DbContext))
                {
                    foreach (var deleted in deleteds)
                    {
                        result = _attributeRepository.Delete(deleted);
                        //删除依赖项
                        _dependencyService.Delete(deleted.AttributeId);
                        //remove from cache
                        _cacheService.RemoveEntity(deleted);
                        //localization
                        _localizedLabelService.DeleteByObject(deleted.AttributeId);
                        _eventPublisher.Publish(new ObjectDeletedEvent<Domain.Attribute>(AttributeDefaults.ModuleName, deleted));
                    }
                    //cascade delete
                    _cascadeDeletes?.ToList().ForEach((x) => { x.CascadeDelete(deleteds.ToArray()); });
                    //update db view
                    _metadataService.AlterView(_entityFinder.FindById(deleteds.First().EntityId));
                }
            }
            return result;
        }
    }
}