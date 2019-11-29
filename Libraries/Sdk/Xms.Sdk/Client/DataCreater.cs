using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core;
using Xms.Core.Data;
using Xms.Event.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Organization;
using Xms.Plugin;
using Xms.Schema.Abstractions;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Data;
using Xms.Sdk.Event;
using Xms.Sdk.Extensions;

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 数据创建服务
    /// </summary>
    public class DataCreater : DataProviderBase, IDataCreater
    {
        private readonly IOrganizationDataProvider _organizationDataProvider;
        private readonly IEntityPluginExecutor _entityPluginExecutor;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDataAssigner _dataAssigner;
        private readonly IFormulaUpdater _formulaUpdater;
        private readonly IMapUpdater _mapUpdater;
        private readonly IEntityValidator _entityValidator;

        public DataCreater(
            IAppContext appContext
            , IEntityFinder entityFinder
            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService
            , IPrincipalObjectAccessService principalObjectAccessService
            , IEventPublisher eventPublisher
            , IBusinessUnitService businessUnitService
            , IOrganizationDataProvider organizationDataProvider
            , IEntityPluginExecutor entityPluginExecutor
            , IAttributeFinder attributeFinder
            , IDataAssigner dataAssigner
            , IFormulaUpdater formulaUpdater
            , IMapUpdater mapUpdater
            , IEntityValidator entityValidator)
            : base(appContext, entityFinder, roleObjectAccessEntityPermissionService, principalObjectAccessService, eventPublisher, businessUnitService)
        {
            _organizationDataProvider = organizationDataProvider;
            _entityPluginExecutor = entityPluginExecutor;
            _attributeFinder = attributeFinder;
            _dataAssigner = dataAssigner;
            _formulaUpdater = formulaUpdater;
            _mapUpdater = mapUpdater;
            _entityValidator = entityValidator;
        }

        public Guid Create(Entity entity, bool ignorePermissions = false)
        {
            var entityMetadata = GetEntityMetaData(entity.Name);
            var attributeMetadatas = _attributeFinder.FindByEntityId(entityMetadata.EntityId);
            VerifyCreate(entity, entityMetadata, attributeMetadatas, ignorePermissions);
            Guid id = Guid.Empty;
            try
            {
                _organizationDataProvider.BeginTransaction();
                InternalOnCreate(entity, OperationStage.PreOperation, entityMetadata, attributeMetadatas);
                id = _organizationDataProvider.Create(entity);
                if (entityMetadata.EntityMask == EntityMaskEnum.User)
                {
                    var owner = entity["ownerid"] as OwnerObject;
                    //assign to other
                    if (!owner.OwnerId.Equals(this._user.SystemUserId))
                    {
                        _dataAssigner.Assign(entityMetadata.EntityId, id, owner);
                    }
                }
                if (!id.Equals(Guid.Empty))
                {
                    var existsData = entity.UnWrapAttributeValue();
                    _mapUpdater.Update(entityMetadata, existsData);
                    //字段表达式计算
                    _formulaUpdater.Update(entityMetadata, existsData);
                }
                _organizationDataProvider.CommitTransaction();
                InternalOnCreate(entity, OperationStage.PostOperation, entityMetadata, attributeMetadatas);
            }
            catch (Exception e)
            {
                _organizationDataProvider.RollBackTransaction();
                id = Guid.Empty;
                OnException(new XmsException(e));
            }
            return id;
        }

        public bool CreateMany(IList<Entity> entities, bool ignorePermissions = false)
        {
            Guard.NotEmpty(entities, nameof(entities));
            var entityMetadata = GetEntityMetaData(entities.First().Name);
            var attributeMetadatas = _attributeFinder.FindByEntityId(entityMetadata.EntityId);
            bool result;
            try
            {
                _organizationDataProvider.BeginTransaction();
                foreach (var entity in entities)
                {
                    VerifyCreate(entity, entityMetadata, attributeMetadatas, ignorePermissions);
                    InternalOnCreate(entity, OperationStage.PreOperation, entityMetadata, attributeMetadatas);
                }
                result = _organizationDataProvider.CreateMany(entities);
                if (result)
                {
                    var entity = entities.Last();
                    //汇总字段
                    _formulaUpdater.Update(entityMetadata, entity);
                    _organizationDataProvider.CommitTransaction();
                    foreach (var item in entities)
                    {
                        InternalOnCreate(item, OperationStage.PostOperation, entityMetadata, attributeMetadatas);
                    }
                }
                else
                {
                    _organizationDataProvider.RollBackTransaction();
                }
            }
            catch (Exception e)
            {
                _organizationDataProvider.RollBackTransaction();
                return OnException(e);
            }
            return result;
        }

        /// <summary>
        /// 校验新建数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityMetadata"></param>
        /// <param name="attributeMetadatas"></param>
        private void VerifyCreate(Entity entity, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas, bool ignorePermissions = false)
        {
            _entityValidator.VerifyValues(entity, entityMetadata, attributeMetadatas, (e) =>
            {
                OnException(string.Join("\n", e));
            });
            if (attributeMetadatas.Exists(n => n.Name.IsCaseInsensitiveEqual("createdby")))
            {
                entity["createdby"] = new EntityReference("systemuser", _user.SystemUserId);
            }
            if (attributeMetadatas.Exists(n => n.Name.IsCaseInsensitiveEqual("ownerid")))
            {
                entity.AddIfNotContain("ownerid", new OwnerObject(OwnerTypes.SystemUser, _user.SystemUserId));
            }
            if (attributeMetadatas.Exists(n => n.Name.IsCaseInsensitiveEqual("owningbusinessunit")) && !_user.BusinessUnitId.Equals(Guid.Empty))
            {
                entity["owningbusinessunit"] = new EntityReference("businessunit", _user.BusinessUnitId);
            }
            if (!entity.IdName.IsCaseInsensitiveEqual("organizationid") && attributeMetadatas.Exists(n => n.Name.IsCaseInsensitiveEqual("organizationid")))
            {
                entity["organizationid"] = new EntityReference("organization", _user.OrganizationId);
            }
            if (attributeMetadatas.Exists(n => n.Name.IsCaseInsensitiveEqual("createdon")))
            {
                entity["createdon"] = DateTime.Now;
            }
            var primaryAttr = attributeMetadatas.Find(n => n.TypeIsPrimaryKey());
            if (!entity.ContainsKey(primaryAttr.Name) || entity.GetIdValue().Equals(Guid.Empty))
            {
                entity.SetAttributeValue(primaryAttr.Name, Guid.NewGuid());
            }
            //set attribute default value
            foreach (var item in attributeMetadatas.Where(n => n.DefaultValue.IsNotEmpty()))
            {
                entity.AddIfNotContain(item.Name, item.DefaultValue);
            }
            if (attributeMetadatas.Exists(n => n.Name.IsCaseInsensitiveEqual("statecode")))
            {
                entity.AddIfNotContain("statecode", true);
            }
            if (attributeMetadatas.Exists(n => n.Name.IsCaseInsensitiveEqual("statuscode")))
            {
                entity.AddIfNotContain("statuscode", new OptionSetValue(0));
            }
            if (!ignorePermissions)
            {
                VerifyEntityPermission(entity, AccessRightValue.Create, entityMetadata);
            }
        }

        /// <summary>
        /// 新建记录时执行的方法
        /// 运行于事务中
        /// </summary>
        /// <param name="data"></param>
        /// <param name="stage"></param>
        /// <param name="entityMetadata"></param>
        public virtual void OnCreate(Entity data, OperationStage stage, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas) { }

        /// <summary>
        /// 创建记录时触发的事件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="stage"></param>
        /// <param name="entityMetadata"></param>
        private void InternalOnCreate(Entity data, OperationStage stage, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas)
        {
            //plugin
            _entityPluginExecutor.Execute(OperationTypeEnum.Create, stage, data, entityMetadata, attributeMetadatas);
            if (stage == OperationStage.PreOperation)
            {
                //event publishing
                _eventPublisher.Publish(new EntityCreatingEvent(data) { EntityMetadata = entityMetadata, AttributeMetadatas = attributeMetadatas });
            }
            else if (stage == OperationStage.PostOperation)
            {
                //event publishing
                _eventPublisher.Publish(new EntityCreatedEvent(data) { EntityMetadata = entityMetadata, AttributeMetadatas = attributeMetadatas });
            }
            OnCreate(data, stage, entityMetadata, attributeMetadatas);
        }
    }
}