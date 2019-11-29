using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Localization.Abstractions;
using Xms.Schema.Abstractions;
using Xms.Schema.Data;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Schema.OptionSet;
using Xms.Schema.RelationShip;
using Xms.Schema.StringMap;
using Xms.Solution.Abstractions;

namespace Xms.Schema.Attribute
{
    /// <summary>
    /// 字段创建服务
    /// </summary>
    public class AttributeCreater : IAttributeCreater
    {
        private readonly IAttributeRepository _attributeRepository;
        private readonly IDefaultAttributeProvider _defaultAttributeProvider;
        private readonly IEntityFinder _entityFinder;
        private readonly IRelationShipCreater _relationShipCreater;
        private readonly IOptionSetCreater _optionSetCreater;
        private readonly IStringMapCreater _stringMapCreater;
        private readonly Caching.CacheManager<Domain.Attribute> _cacheService;
        private readonly ILocalizedLabelBatchBuilder _localizedLabelService;
        private readonly ILocalizedTextProvider _loc;
        private readonly IMetadataService _metadataService;
        private readonly IAttributeDependency _dependencyService;
        private readonly IAppContext _appContext;

        public AttributeCreater(IAppContext appContext
            , IAttributeRepository attributeRepository
            , IEntityFinder entityFinder
            , IRelationShipCreater relationShipCreater
            , IOptionSetCreater optionSetCreater
            , IStringMapCreater stringMapCreater
            , ILocalizedLabelBatchBuilder localizedLabelService
            , IMetadataService metadataService
            , IDefaultAttributeProvider defaultAttributeProvider
            , IAttributeDependency dependencyService)
        {
            _appContext = appContext;
            _attributeRepository = attributeRepository;
            _loc = _appContext.GetFeature<ILocalizedTextProvider>();
            _localizedLabelService = localizedLabelService;
            _entityFinder = entityFinder;
            _relationShipCreater = relationShipCreater;
            _optionSetCreater = optionSetCreater;
            _stringMapCreater = stringMapCreater;
            _cacheService = new Caching.CacheManager<Domain.Attribute>(_appContext.OrganizationUniqueName + ":attributes", _appContext.PlatformSettings.CacheEnabled);
            _metadataService = metadataService;
            _defaultAttributeProvider = defaultAttributeProvider;
            _dependencyService = dependencyService;
        }

        public bool Create(Domain.Attribute entity)
        {
            //检查名称是否已存在
            if (_attributeRepository.Exists(x => x.EntityId == entity.EntityId && x.Name == entity.Name))
            {
                throw new XmsException(_loc["ATTRIBUTE_NAME_EXISTS"]);
            }
            var result = true;
            if (entity.DefaultValue.IsEmpty() && (entity.TypeIsBit() || entity.TypeIsDecimal() || entity.TypeIsFloat()
                || entity.TypeIsInt() || entity.TypeIsMoney() || entity.TypeIsSmallInt() || entity.TypeIsSmallMoney() || entity.TypeIsState() || entity.TypeIsStatus()))
            {
                entity.DefaultValue = "0";
            }
            var parentEntity = _entityFinder.FindById(entity.EntityId);
            using (UnitOfWork.Build(_attributeRepository.DbContext))
            {
                result = _attributeRepository.Create(entity);
                if (result)
                {
                    //引用类型
                    if (entity.TypeIsLookUp() || entity.TypeIsOwner() || entity.TypeIsCustomer())
                    {
                        var referencing = _entityFinder.FindById(entity.EntityId);
                        var referenced = _attributeRepository.Find(x => x.EntityId == entity.ReferencedEntityId.Value && x.AttributeTypeName == AttributeTypeIds.PRIMARYKEY);
                        var relationShip = new Domain.RelationShip
                        {
                            Name = "lk_" + referencing.Name + "_" + entity.Name,
                            RelationshipType = RelationShipType.ManyToOne,
                            ReferencingAttributeId = entity.AttributeId,
                            ReferencingEntityId = entity.EntityId,
                            ReferencedAttributeId = referenced.AttributeId,
                            ReferencedEntityId = referenced.EntityId,
                            CascadeLinkMask = parentEntity.ParentEntityId.HasValue ? 1 : 2,
                            CascadeAssign = parentEntity.ParentEntityId.HasValue ? 1 : 4,
                            CascadeDelete = parentEntity.ParentEntityId.HasValue ? 1 : 4,
                            CascadeShare = parentEntity.ParentEntityId.HasValue ? 1 : 4,
                            CascadeUnShare = parentEntity.ParentEntityId.HasValue ? 1 : 4,
                            IsCustomizable = !parentEntity.ParentEntityId.HasValue
                        };
                        _relationShipCreater.Create(relationShip);
                    }
                    //选项类型
                    else if (entity.TypeIsPickList())
                    {
                        if (entity.OptionSet != null && !entity.OptionSet.IsPublic)
                        {
                            _optionSetCreater.Create(entity.OptionSet);
                        }
                    }
                    //bit类型
                    else if (entity.TypeIsBit())
                    {
                        _stringMapCreater.CreateMany(entity.PickLists);
                    }
                    //update db view
                    _metadataService.AlterView(_entityFinder.FindById(entity.EntityId));
                    //依赖项
                    _dependencyService.Create(entity);
                    //本地化标签
                    _localizedLabelService.Append(SolutionDefaults.DefaultSolutionId, entity.LocalizedName.IfEmpty(""), AttributeDefaults.ModuleName, "LocalizedName", entity.AttributeId)
                    .Append(SolutionDefaults.DefaultSolutionId, entity.Description.IfEmpty(""), AttributeDefaults.ModuleName, "Description", entity.AttributeId)
                    .Save();

                    //add to cache
                    _cacheService.SetEntity(entity);
                }
            }
            return result;
        }

        public bool CreateDefaultAttributes(Domain.Entity entity, params string[] defaultAttributeNames)
        {
            var result = true;
            var attributes = _defaultAttributeProvider.GetSysAttributes(entity).Where(x => defaultAttributeNames.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase)).ToList();
            using (UnitOfWork.Build(_attributeRepository.DbContext))
            {
                result = _attributeRepository.CreateMany(attributes);
                //新建字段选项
                var stateCodeAttr = attributes.Find(n => n.Name.IsCaseInsensitiveEqual("statecode"));
                if (stateCodeAttr != null)
                {
                    result = _stringMapCreater.CreateMany(stateCodeAttr.PickLists);
                }
                var statusCodeAttr = attributes.Find(n => n.Name.IsCaseInsensitiveEqual("statuscode"));
                if (statusCodeAttr != null)
                {
                    _optionSetCreater.Create(statusCodeAttr.OptionSet);
                }
                //创建关系
                result = _relationShipCreater.CreateMany(_defaultAttributeProvider.GetSysAttributeRelationShips(entity, attributes));
                //attribute localization
                foreach (var attr in attributes)
                {
                    var label = attr.TypeIsPrimaryKey() ? entity.LocalizedName : _loc["entity_sys_" + attr.Name];
                    _localizedLabelService.Append(entity.SolutionId, label, AttributeDefaults.ModuleName, "LocalizedName", attr.AttributeId)
                        .Append(entity.SolutionId, label, AttributeDefaults.ModuleName, "Description", attr.AttributeId);
                }
                _localizedLabelService.Save();
                //add to cache
                foreach (var attr in attributes)
                {
                    _cacheService.SetEntity(attr);
                }
            }
            return result;
        }

        public bool CreateOwnerAttributes(Domain.Entity entity)
        {
            var existAttributes = _attributeRepository.Query(f => f.EntityId == entity.EntityId && f.Name.In("OwnerId", "OwnerIdType", "OwningBusinessUnit"));
            if (existAttributes != null && existAttributes.Count() == 3)
            {
                return true;
            }
            var systemUserPrimaryKey = _attributeRepository.Find(x => x.EntityName == "SystemUser" && x.Name == "SystemUserId");
            var businessUnitPrimaryKey = _attributeRepository.Find(x => x.EntityName == "BusinessUnit" && x.Name == "BusinessUnitId");
            List<Domain.Attribute> attrList = new List<Domain.Attribute>();
            if (!existAttributes.Exists(x => x.Name.IsCaseInsensitiveEqual("OwnerId")))
            {
                attrList.Add(new Domain.Attribute() { AttributeId = Guid.NewGuid(), EntityId = entity.EntityId, Name = "OwnerId", LocalizedName = _loc["entity_sys_ownerid"], AttributeTypeName = AttributeTypeIds.OWNER, EntityName = entity.Name, ReferencedEntityId = systemUserPrimaryKey.EntityId, IsNullable = false, IsRequired = true, LogEnabled = false, IsCustomizable = false, IsCustomField = false });
            }
            if (!existAttributes.Exists(x => x.Name.IsCaseInsensitiveEqual("OwnerIdType")))
            {
                attrList.Add(new Domain.Attribute() { AttributeId = Guid.NewGuid(), EntityId = entity.EntityId, Name = "OwnerIdType", LocalizedName = _loc["entity_sys_owneridtype"], AttributeTypeName = AttributeTypeIds.INT, EntityName = entity.Name, IsNullable = false, IsRequired = true, LogEnabled = false, IsCustomizable = false, IsCustomField = false });
            }
            if (!existAttributes.Exists(x => x.Name.IsCaseInsensitiveEqual("OwningBusinessUnit")))
            {
                attrList.Add(new Domain.Attribute() { AttributeId = Guid.NewGuid(), EntityId = entity.EntityId, Name = "OwningBusinessUnit", LocalizedName = _loc["entity_sys_owningbusinessunit"], AttributeTypeName = AttributeTypeIds.LOOKUP, EntityName = entity.Name, ReferencedEntityId = businessUnitPrimaryKey.EntityId, IsNullable = true, IsRequired = true, LogEnabled = false, IsCustomizable = false, IsCustomField = false });
            }
            foreach (var attr in attrList)
            {
                Create(attr);
            }
            return true;
        }

        public bool CreateWorkFlowAttributes(Domain.Entity entity)
        {
            var existAttributes = _attributeRepository.Query(f => f.EntityId == entity.EntityId && f.Name.In("WorkFlowId", "ProcessState"));
            if (existAttributes != null && existAttributes.Count() == 2)
            {
                return true;
            }
            var workFlowPrimaryKey = entity.WorkFlowEnabled ? _attributeRepository.Find(x => x.EntityName == "WorkFlow" && x.Name == "WorkFlowId") : null;
            List<Domain.Attribute> attributes = new List<Domain.Attribute>();
            if (!existAttributes.Exists(x => x.Name.IsCaseInsensitiveEqual("WorkFlowId")))
            {
                attributes.Add(new Domain.Attribute() { AttributeId = Guid.NewGuid(), EntityId = entity.EntityId, Name = "WorkFlowId", LocalizedName = _loc["entity_sys_workflowid"], AttributeTypeName = AttributeTypeIds.LOOKUP, EntityName = entity.Name, ReferencedEntityId = workFlowPrimaryKey.EntityId, IsNullable = true, IsRequired = false, LogEnabled = false, IsCustomizable = false, IsCustomField = false });
            }

            if (!existAttributes.Exists(x => x.Name.IsCaseInsensitiveEqual("ProcessState")))
            {
                attributes.Add(new Domain.Attribute() { AttributeId = Guid.NewGuid(), EntityId = entity.EntityId, Name = "ProcessState", LocalizedName = _loc["entity_sys_processstate"], AttributeTypeName = AttributeTypeIds.PICKLIST, EntityName = entity.Name, OptionSetId = Guid.Parse("00000000-0000-0000-0000-000000000001"), IsNullable = true, IsRequired = false, LogEnabled = false, IsCustomizable = false, IsCustomField = false, DefaultValue = "-1" });
            }
            foreach (var attr in attributes)
            {
                Create(attr);
            }
            return true;
        }

        public bool CreateBusinessFlowAttributes(Domain.Entity entity)
        {
            var existAttributes = _attributeRepository.Query(f => f.EntityId == entity.EntityId && f.Name.In("StageId"));
            if (existAttributes != null && existAttributes.Count() == 1)
            {
                return true;
            }
            var attr = new Domain.Attribute() { AttributeId = Guid.NewGuid(), EntityId = entity.EntityId, Name = "StageId", LocalizedName = _loc["entity_sys_processstage"], AttributeTypeName = AttributeTypeIds.UNIQUEIDENTIFIER, EntityName = entity.Name, IsNullable = true, IsRequired = false, LogEnabled = false, IsCustomizable = false, IsCustomField = false };
            return Create(attr);
        }
    }
}