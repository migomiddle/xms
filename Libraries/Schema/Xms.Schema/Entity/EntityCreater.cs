using System;
using System.Linq;
using Xms.Context;
using Xms.Data.Abstractions;
using Xms.Event.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Localization.Abstractions;
using Xms.Schema.Abstractions;
using Xms.Schema.Attribute;
using Xms.Schema.Data;
using Xms.Solution;
using Xms.Solution.Abstractions;

namespace Xms.Schema.Entity
{
    /// <summary>
    /// 实体创建服务
    /// </summary>
    public class EntityCreater : IEntityCreater
    {
        private readonly IEntityRepository _entityRepository;
        private readonly ILocalizedLabelBatchBuilder _localizedLabelService;
        private readonly Caching.CacheManager<Domain.Entity> _cacheService;
        private readonly ISolutionComponentService _solutionComponentService;

        private readonly IDefaultAttributeProvider _defaultAttributeProvider;
        private readonly IAttributeCreater _attributeCreater;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAppContext _appContext;
        private readonly ILocalizedTextProvider _loc;

        public EntityCreater(IAppContext appContext
            , IEntityRepository entityRepository
            , ILocalizedLabelBatchBuilder localizedLabelService
            , ISolutionComponentService solutionComponentService
            , IDefaultAttributeProvider defaultAttributeProvider
            , IAttributeCreater attributeCreater
            , IEventPublisher eventPublisher
            )
        {
            _appContext = appContext;
            _loc = _appContext.GetFeature<ILocalizedTextProvider>();
            _entityRepository = entityRepository;
            _localizedLabelService = localizedLabelService;
            _solutionComponentService = solutionComponentService;
            _defaultAttributeProvider = defaultAttributeProvider;
            _attributeCreater = attributeCreater;
            _cacheService = new Caching.CacheManager<Domain.Entity>(_appContext.OrganizationUniqueName + ":entities", _appContext.PlatformSettings.CacheEnabled);
            _eventPublisher = eventPublisher;
        }

        public bool Create(Domain.Entity entity, bool makeAllDefaultAttributes = true)
        {
            return this.Create(entity, AttributeDefaults.SystemAttributes);
        }

        public bool Create(Domain.Entity entity, params string[] defaultAttributeNames)
        {
            if (_entityRepository.Exists(x => x.Name == entity.Name))
            {
                throw new XmsException(_loc["name_already_exists"]);
            }
            var solutionid = entity.SolutionId;//当前解决方案
            entity.SolutionId = SolutionDefaults.DefaultSolutionId;//组件属于默认解决方案
            var result = true;
            if (defaultAttributeNames.IsEmpty())
            {
                //默认添加主键字段
                defaultAttributeNames = new string[] { entity.Name + "Id" };
            }
            else if (!defaultAttributeNames.Contains(entity.Name + "Id", StringComparer.InvariantCultureIgnoreCase))
            {
                var namesList = defaultAttributeNames.ToList();
                namesList.Add(entity.Name + "Id");
                defaultAttributeNames = namesList.ToArray();
            }
            var parentEntity = entity.ParentEntityId.HasValue ? _entityRepository.FindById(entity.ParentEntityId.Value) : null;
            var defaultAttributes = _defaultAttributeProvider.GetSysAttributes(entity).Where(x => defaultAttributeNames.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase)).Distinct().ToList();
            using (UnitOfWork.Build(_entityRepository.DbContext))
            {
                result = _entityRepository.Create(entity, defaultAttributes, _defaultAttributeProvider.GetSysAttributeRelationShips(entity, defaultAttributes));

                //创建默认字段
                _attributeCreater.CreateDefaultAttributes(entity, defaultAttributeNames);
                //如果是子实体，则创建引用字段
                if (parentEntity != null)
                {
                    _attributeCreater.Create(new Domain.Attribute
                    {
                        Name = parentEntity.Name + "Id"
                        ,
                        AttributeTypeName = AttributeTypeIds.LOOKUP
                        ,
                        EntityId = entity.EntityId
                        ,
                        EntityName = entity.Name
                        ,
                        IsRequired = true
                        ,
                        LocalizedName = parentEntity.LocalizedName
                        ,
                        ReferencedEntityId = parentEntity.EntityId
                    });
                }
                //事件发布
                _eventPublisher.Publish(new ObjectCreatedEvent<Domain.Entity>(entity));
                //solution component
                _solutionComponentService.Create(solutionid, entity.EntityId, EntityDefaults.ModuleName);
                //本地化标签
                _localizedLabelService.Append(entity.SolutionId, entity.LocalizedName.IfEmpty(""), EntityDefaults.ModuleName, "LocalizedName", entity.EntityId)
                .Append(entity.SolutionId, entity.Description.IfEmpty(""), EntityDefaults.ModuleName, "Description", entity.EntityId)
                .Save();

                //add to cache
                _cacheService.SetEntity(entity);
            }
            return result;
        }
    }
}