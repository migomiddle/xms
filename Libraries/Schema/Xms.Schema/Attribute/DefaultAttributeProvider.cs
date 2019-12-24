using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Core.Data;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Schema.Abstractions;

namespace Xms.Schema.Attribute
{
    /// <summary>
    /// 系统默认字段服务
    /// </summary>
    public class DefaultAttributeProvider : IDefaultAttributeProvider
    {
        private readonly IAttributeFinder _attributeFinder;
        private readonly ILocalizedTextProvider _loc;
        private readonly IAppContext _appContext;
        private readonly ICurrentUser _currentUser;

        public DefaultAttributeProvider(IAppContext appContext
            , IAttributeFinder attributeFinder)
        {
            _appContext = appContext;
            _attributeFinder = attributeFinder;
            _loc = _appContext.GetFeature<ILocalizedTextProvider>();
            _currentUser = _appContext.GetFeature<ICurrentUser>();
        }

        public List<Domain.Attribute> GetSysAttributes(Domain.Entity entity)
        {
            var primaryFields = _attributeFinder.Query(x => x.Where(f => f.EntityName.In("SystemUser", "BusinessUnit", "Organization", "WorkFlow") && f.Name.In("SystemUserId", "BusinessUnitId", "OrganizationId", "WorkFlowId")));
            var systemUserPrimaryKey = primaryFields.Find(x => x.Name == "SystemUserId");
            var businessUnitPrimaryKey = primaryFields.Find(x => x.Name == "BusinessUnitId");
            var organizationPrimaryKey = primaryFields.Find(x => x.Name == "OrganizationId");
            var workFlowPrimaryKey = primaryFields.Find(x => x.Name == "WorkFlowId");
            //插入自动创建的字段
            List<Domain.Attribute> attrList = new List<Domain.Attribute>();
            var primaryAttr = new Domain.Attribute
            {
                EntityId = entity.EntityId,
                Name = entity.Name + "Id",
                LocalizedName = entity.LocalizedName,
                AttributeTypeName = AttributeTypeIds.PRIMARYKEY,
                EntityName = entity.Name,
                IsNullable = false,
                IsRequired = true,
                LogEnabled = false,
                DefaultValue = "newid()",
                IsCustomizable = false,
                IsCustomField = false,
                CreatedBy = _currentUser.SystemUserId
            };
            attrList.Add(primaryAttr);
            var nameAttr = new Domain.Attribute
            {
                EntityId = entity.EntityId,
                Name = "Name",
                LocalizedName = _loc["entity_sys_name"],
                AttributeTypeName = AttributeTypeIds.NVARCHAR,
                EntityName = entity.Name,
                IsNullable = true,
                IsRequired = true,
                LogEnabled = false,
                MaxLength = 300,
                IsCustomField = false,
                IsPrimaryField = true,
                CreatedBy = _currentUser.SystemUserId
            };
            attrList.Add(nameAttr);
            var vnAttr = new Domain.Attribute
            {
                EntityId = entity.EntityId,
                Name = "VersionNumber",
                LocalizedName = _loc["entity_sys_versionnumber"],
                AttributeTypeName = AttributeTypeIds.TIMESTAMP,
                EntityName = entity.Name,
                IsNullable = false,
                IsRequired = true,
                LogEnabled = false,
                IsCustomizable = false,
                IsCustomField = false,
                CreatedBy = _currentUser.SystemUserId
            };
            attrList.Add(vnAttr);
            var createdOnAttr = new Domain.Attribute
            {
                EntityId = entity.EntityId,
                Name = "CreatedOn",
                LocalizedName = _loc["entity_sys_createdon"],
                AttributeTypeName = AttributeTypeIds.DATETIME,
                EntityName = entity.Name,
                IsNullable = false,
                IsRequired = true,
                LogEnabled = false,
                IsCustomizable = false,
                IsCustomField = false,
                CreatedBy = _currentUser.SystemUserId
            };
            attrList.Add(createdOnAttr);
            var createdByAttr = new Domain.Attribute
            {
                EntityId = entity.EntityId,
                Name = "CreatedBy",
                LocalizedName = _loc["entity_sys_createdby"],
                AttributeTypeName = AttributeTypeIds.LOOKUP,
                EntityName = entity.Name,
                ReferencedEntityId = systemUserPrimaryKey.EntityId,
                IsNullable = false,
                IsRequired = true,
                LogEnabled = false,
                IsCustomizable = false,
                IsCustomField = false,
                CreatedBy = _currentUser.SystemUserId
            };
            attrList.Add(createdByAttr);
            var modifiedOnAttr = new Domain.Attribute
            {
                EntityId = entity.EntityId,
                Name = "ModifiedOn",
                LocalizedName = _loc["entity_sys_modifiedon"],
                AttributeTypeName = AttributeTypeIds.DATETIME,
                EntityName = entity.Name,
                IsNullable = true,
                IsRequired = false,
                LogEnabled = false,
                IsCustomizable = false,
                IsCustomField = false,
                CreatedBy = _currentUser.SystemUserId
            };
            attrList.Add(modifiedOnAttr);
            var modifiedByAttr = new Domain.Attribute
            {
                EntityId = entity.EntityId,
                Name = "ModifiedBy",
                LocalizedName = _loc["entity_sys_modifiedby"],
                AttributeTypeName = AttributeTypeIds.LOOKUP,
                EntityName = entity.Name,
                ReferencedEntityId = systemUserPrimaryKey.EntityId,
                IsNullable = true,
                IsRequired = false,
                LogEnabled = false,
                IsCustomizable = false,
                IsCustomField = false,
                CreatedBy = _currentUser.SystemUserId
            };
            attrList.Add(modifiedByAttr);

            #region 状态字段

            var stateCodeAttr = new Domain.Attribute
            {
                EntityId = entity.EntityId,
                Name = "StateCode",
                LocalizedName = _loc["entity_sys_statecode"],
                AttributeTypeName = AttributeTypeIds.STATE,
                EntityName = entity.Name,
                IsNullable = false,
                IsRequired = true,
                LogEnabled = false,
                IsCustomizable = false,
                IsCustomField = false,
                CreatedBy = _currentUser.SystemUserId
            };
            List<Domain.StringMap> statePicklist = new List<Domain.StringMap>();
            var stateCodeMap = new Domain.StringMap
            {
                StringMapId = Guid.NewGuid(),
                AttributeId = stateCodeAttr.AttributeId,
                Name = _loc["enabled"],
                Value = 1,
                DisplayOrder = 0,
                AttributeName = "StateCode",
                EntityName = entity.Name
            };
            statePicklist.Add(stateCodeMap);
            var stateCodeMap2 = new Domain.StringMap
            {
                StringMapId = Guid.NewGuid(),
                AttributeId = stateCodeAttr.AttributeId,
                Name = _loc["disabled"],
                Value = 0,
                DisplayOrder = 1,
                AttributeName = "StateCode",
                EntityName = entity.Name
            };
            statePicklist.Add(stateCodeMap2);
            stateCodeAttr.PickLists = statePicklist;
            attrList.Add(stateCodeAttr);

            #endregion 状态字段

            #region 状态描述字段

            var statusCodeAttr = new Domain.Attribute
            {
                EntityId = entity.EntityId,
                Name = "StatusCode",
                LocalizedName = _loc["entity_sys_statuscode"],
                AttributeTypeName = AttributeTypeIds.STATUS,
                EntityName = entity.Name,
                IsNullable = false,
                IsRequired = true,
                LogEnabled = false,
                IsCustomizable = false,
                IsCustomField = false,
                DefaultValue = "0",
                CreatedBy = _currentUser.SystemUserId
            };

            var os = new Domain.OptionSet
            {
                ComponentState = entity.ComponentState,
                SolutionId = entity.SolutionId,
                CreatedBy = statusCodeAttr.CreatedBy,
                IsPublic = false,
                Name = statusCodeAttr.LocalizedName
            };
            var osDetail1 = new Domain.OptionSetDetail
            {
                DisplayOrder = 0,
                IsSelected = false,
                Name = _loc["attribute_statuscode_draft"],
                Value = 0,
                OptionSetId = os.OptionSetId
            };
            var osDetail2 = new Domain.OptionSetDetail
            {
                DisplayOrder = 1,
                IsSelected = false,
                Name = _loc["attribute_statuscode_actived"],
                Value = 1,
                OptionSetId = os.OptionSetId
            };
            var osDetail3 = new Domain.OptionSetDetail
            {
                DisplayOrder = 2,
                IsSelected = false,
                Name = _loc["attribute_statuscode_invalid"],
                Value = 2,
                OptionSetId = os.OptionSetId
            };
            os.Items = new List<Domain.OptionSetDetail>() { osDetail1, osDetail2, osDetail3 };
            statusCodeAttr.OptionSet = os;
            statusCodeAttr.OptionSetId = os.OptionSetId;
            attrList.Add(statusCodeAttr);

            #endregion 状态描述字段

            //实体范围
            if (entity.EntityMask == EntityMaskEnum.User)
            {
                var ownerIdAttr = new Domain.Attribute
                {
                    EntityId = entity.EntityId,
                    Name = "OwnerId",
                    LocalizedName = _loc["entity_sys_ownerid"],
                    AttributeTypeName = AttributeTypeIds.OWNER,
                    EntityName = entity.Name,
                    ReferencedEntityId = systemUserPrimaryKey.EntityId,
                    IsNullable = false,
                    IsRequired = true,
                    LogEnabled = false,
                    IsCustomizable = false,
                    IsCustomField = false
                };
                attrList.Add(ownerIdAttr);
                var ownerIdTypeAttr = new Domain.Attribute
                {
                    EntityId = entity.EntityId,
                    Name = "OwnerIdType",
                    LocalizedName = _loc["entity_sys_owneridtype"],
                    AttributeTypeName = AttributeTypeIds.INT,
                    EntityName = entity.Name,
                    IsNullable = false,
                    IsRequired = true,
                    LogEnabled = false,
                    IsCustomizable = false,
                    IsCustomField = false
                };
                attrList.Add(ownerIdTypeAttr);
                var owningBusinessUnitAttr = new Domain.Attribute
                {
                    EntityId = entity.EntityId,
                    Name = "OwningBusinessUnit",
                    LocalizedName = _loc["entity_sys_owningbusinessunit"],
                    AttributeTypeName = AttributeTypeIds.LOOKUP,
                    EntityName = entity.Name,
                    ReferencedEntityId = businessUnitPrimaryKey.EntityId,
                    IsNullable = true,
                    IsRequired = true,
                    LogEnabled = false,
                    IsCustomizable = false,
                    IsCustomField = false
                };
                attrList.Add(owningBusinessUnitAttr);
            }
            var organizationIdAttr = new Domain.Attribute
            {
                EntityId = entity.EntityId,
                Name = "OrganizationId",
                LocalizedName = _loc["entity_sys_organizationid"],
                AttributeTypeName = AttributeTypeIds.LOOKUP,
                EntityName = entity.Name,
                ReferencedEntityId = organizationPrimaryKey.EntityId,
                IsNullable = false,
                IsRequired = true,
                LogEnabled = false,
                IsCustomizable = false,
                IsCustomField = false
            };
            attrList.Add(organizationIdAttr);
            //启用审批流
            if (entity.WorkFlowEnabled)
            {
                var workflowIdAttr = new Domain.Attribute
                {
                    EntityId = entity.EntityId,
                    Name = "WorkFlowId",
                    LocalizedName = _loc["entity_sys_workflowid"],
                    AttributeTypeName = AttributeTypeIds.LOOKUP,
                    EntityName = entity.Name,
                    ReferencedEntityId = workFlowPrimaryKey.EntityId,
                    IsNullable = true,
                    IsRequired = false,
                    LogEnabled = false,
                    IsCustomizable = false,
                    IsCustomField = false
                };
                attrList.Add(workflowIdAttr);
                var processStateAttr = new Domain.Attribute
                {
                    EntityId = entity.EntityId,
                    Name = "ProcessState",
                    LocalizedName = _loc["entity_sys_processstate"],
                    AttributeTypeName = AttributeTypeIds.PICKLIST,
                    EntityName = entity.Name,
                    OptionSetId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    IsNullable = true,
                    IsRequired = false,
                    LogEnabled = false,
                    IsCustomizable = false,
                    IsCustomField = false,
                    DefaultValue = "-1"
                };
                attrList.Add(processStateAttr);
            }
            //启用业务流程
            if (entity.BusinessFlowEnabled)
            {
                var stageIdAttr = new Domain.Attribute
                {
                    EntityId = entity.EntityId,
                    Name = "StageId",
                    LocalizedName = _loc["entity_sys_processstage"],
                    AttributeTypeName = AttributeTypeIds.UNIQUEIDENTIFIER,
                    EntityName = entity.Name,
                    IsNullable = true,
                    IsRequired = false,
                    LogEnabled = false,
                    IsCustomizable = false,
                    IsCustomField = false
                };
                attrList.Add(stageIdAttr);
            }
            return attrList;
        }

        public List<Domain.RelationShip> GetSysAttributeRelationShips(Domain.Entity entity, List<Domain.Attribute> sysAttributes)
        {
            var primaryFields = _attributeFinder.Query(x => x.Where(f => f.EntityName.In("SystemUser", "BusinessUnit", "Organization", "WorkFlow") && f.Name.In("SystemUserId", "BusinessUnitId", "OrganizationId", "WorkFlowId")));
            var systemUserPrimaryKey = primaryFields.Find(x => x.Name == "SystemUserId");
            var businessUnitPrimaryKey = primaryFields.Find(x => x.Name == "BusinessUnitId");
            var organizationPrimaryKey = primaryFields.Find(x => x.Name == "OrganizationId");
            var workFlowPrimaryKey = primaryFields.Find(x => x.Name == "WorkFlowId");
            List<Domain.RelationShip> relationships = new List<Domain.RelationShip>();
            if (sysAttributes.Exists(n => n.Name.IsCaseInsensitiveEqual("CreatedBy")))
            {
                var createdByRelationShip = new Domain.RelationShip
                {
                    Name = "lk_" + entity.Name + "_createdby",
                    RelationshipType = RelationShipType.ManyToOne,
                    ReferencingAttributeId = sysAttributes.Find(n => n.Name.IsCaseInsensitiveEqual("CreatedBy")).AttributeId,
                    ReferencingAttributeName = "CreatedBy",
                    ReferencedEntityName = "SystemUser",
                    ReferencedAttributeName = "SystemUserId",
                    ReferencingEntityId = entity.EntityId,
                    ReferencedAttributeId = systemUserPrimaryKey.AttributeId,
                    ReferencedEntityId = systemUserPrimaryKey.EntityId
                };
                relationships.Add(createdByRelationShip);
            }
            if (sysAttributes.Exists(n => n.Name.IsCaseInsensitiveEqual("ModifiedBy")))
            {
                var modifiedByRelationShip = new Domain.RelationShip
                {
                    Name = "lk_" + entity.Name + "_modifiedby",
                    RelationshipType = RelationShipType.ManyToOne,
                    ReferencingAttributeId = sysAttributes.Find(n => n.Name == "ModifiedBy").AttributeId,
                    ReferencingAttributeName = "ModifiedBy",
                    ReferencedEntityName = "SystemUser",
                    ReferencedAttributeName = "SystemUserId",
                    ReferencingEntityId = entity.EntityId,
                    ReferencedAttributeId = systemUserPrimaryKey.AttributeId,
                    ReferencedEntityId = systemUserPrimaryKey.EntityId
                };
                relationships.Add(modifiedByRelationShip);
            }
            if (entity.EntityMask == EntityMaskEnum.User)
            {
                if (sysAttributes.Exists(n => n.Name.IsCaseInsensitiveEqual("OwnerId")))
                {
                    var ownerIdRelationShip = new Domain.RelationShip
                    {
                        Name = "lk_" + entity.Name + "_ownerid",
                        RelationshipType = RelationShipType.ManyToOne,
                        ReferencingAttributeId = sysAttributes.Find(n => n.Name.IsCaseInsensitiveEqual("OwnerId")).AttributeId,
                        ReferencingAttributeName = "OwnerId",
                        ReferencedEntityName = "SystemUser",
                        ReferencedAttributeName = "SystemUserId",
                        ReferencingEntityId = entity.EntityId,
                        ReferencedAttributeId = systemUserPrimaryKey.AttributeId,
                        ReferencedEntityId = systemUserPrimaryKey.EntityId
                    };
                    relationships.Add(ownerIdRelationShip);
                }
                if (sysAttributes.Exists(n => n.Name.IsCaseInsensitiveEqual("OwningBusinessUnit")))
                {
                    var owningBusinessUnitRelationShip = new Domain.RelationShip
                    {
                        Name = "lk_" + entity.Name + "_owningbusinessunit",
                        RelationshipType = RelationShipType.ManyToOne,
                        ReferencingAttributeId = sysAttributes.Find(n => n.Name.IsCaseInsensitiveEqual("OwningBusinessUnit")).AttributeId,
                        ReferencingAttributeName = "OwningBusinessUnit",
                        ReferencedEntityName = "BusinessUnit",
                        ReferencedAttributeName = "BusinessUnitId",
                        ReferencingEntityId = entity.EntityId,
                        ReferencedAttributeId = businessUnitPrimaryKey.AttributeId,
                        ReferencedEntityId = businessUnitPrimaryKey.EntityId
                    };
                    relationships.Add(owningBusinessUnitRelationShip);
                }
            }
            if (sysAttributes.Exists(n => n.Name.IsCaseInsensitiveEqual("OrganizationId")))
            {
                var organizationIdRelationShip = new Domain.RelationShip
                {
                    Name = "lk_" + entity.Name + "_organizationid",
                    RelationshipType = RelationShipType.ManyToOne,
                    ReferencingAttributeId = sysAttributes.Find(n => n.Name.IsCaseInsensitiveEqual("OrganizationId")).AttributeId,
                    ReferencingAttributeName = "OrganizationId",
                    ReferencedEntityName = "Organization",
                    ReferencedAttributeName = "OrganizationId",
                    ReferencingEntityId = entity.EntityId,
                    ReferencedAttributeId = organizationPrimaryKey.AttributeId,
                    ReferencedEntityId = organizationPrimaryKey.EntityId
                };
                relationships.Add(organizationIdRelationShip);
            }
            if (entity.WorkFlowEnabled)
            {
                if (sysAttributes.Exists(n => n.Name.IsCaseInsensitiveEqual("WorkFlowId")))
                {
                    var workflowIdRelationShip = new Domain.RelationShip
                    {
                        Name = "lk_" + entity.Name + "_workflowid",
                        RelationshipType = RelationShipType.ManyToOne,
                        ReferencingAttributeId = sysAttributes.Find(n => n.Name.IsCaseInsensitiveEqual("WorkFlowId")).AttributeId,
                        ReferencingAttributeName = "WorkFlowId",
                        ReferencedEntityName = "WorkFlow",
                        ReferencedAttributeName = "WorkFlowId",
                        ReferencingEntityId = entity.EntityId,
                        ReferencedAttributeId = workFlowPrimaryKey.AttributeId,
                        ReferencedEntityId = workFlowPrimaryKey.EntityId
                    };
                    relationships.Add(workflowIdRelationShip);
                }
            }
            return relationships;
        }
    }
}