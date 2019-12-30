using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core;
using Xms.Core.Data;
using Xms.Event.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Organization;
using Xms.Plugin;
using Xms.Schema.Abstractions;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Data;
using Xms.Sdk.Event;
using Xms.Sdk.Extensions;
using Xms.Sdk.Query;

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 数据更新服务
    /// </summary>
    public class DataUpdater : DataProviderBase, IDataUpdater
    {
        private readonly IOrganizationDataProvider _organizationDataProvider;
        private readonly IOrganizationDataRetriever _organizationDataRetriever;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IEntityPluginExecutor _entityPluginExecutor;
        private readonly IDataAssigner _dataAssigner;
        private readonly IMapUpdater _mapUpdater;
        private readonly IQueryResolverFactory _queryResolverFactory;
        private readonly IEntityValidator _entityValidator;

        private readonly IFormulaUpdater _formulaUpdater;

        public DataUpdater(
            IAppContext appContext
            , IEntityFinder entityFinder
            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService
            , IPrincipalObjectAccessService principalObjectAccessService
            , IEventPublisher eventPublisher
            , IBusinessUnitService businessUnitService
            , IOrganizationDataProvider organizationDataProvider
            , IOrganizationDataRetriever organizationDataRetriever
            , IAttributeFinder attributeFinder
            , IEntityPluginExecutor entityPluginExecutor
            , IDataAssigner dataAssigner
            , IMapUpdater mapUpdater
            , IFormulaUpdater formulaUpdater
            , IQueryResolverFactory queryResolverFactory
            , IEntityValidator entityValidator
            )
            : base(appContext, entityFinder, roleObjectAccessEntityPermissionService, principalObjectAccessService, eventPublisher, businessUnitService)
        {
            _organizationDataProvider = organizationDataProvider;
            _organizationDataRetriever = organizationDataRetriever;
            _attributeFinder = attributeFinder;
            _entityPluginExecutor = entityPluginExecutor;
            _dataAssigner = dataAssigner;
            _mapUpdater = mapUpdater;
            _formulaUpdater = formulaUpdater;
            _queryResolverFactory = queryResolverFactory;
            _entityValidator = entityValidator;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <param name="ignorePermissions">是否忽略权限</param>
        /// <returns></returns>
        public bool Update(Entity entity, bool ignorePermissions = false)
        {
            var entityMetadata = GetEntityMetaData(entity.Name);
            var attributeMetadatas = _attributeFinder.FindByEntityId(entityMetadata.EntityId);
            VerifyUpdate(entity, entityMetadata, attributeMetadatas);
            var query = new QueryExpression(entity.Name, _languageId);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition(entity.IdName, ConditionOperator.Equal, entity.GetIdValue());
            var originalEntity = _organizationDataRetriever.Retrieve(query, true);
            //验证更新权限
            if (!ignorePermissions)
            {
                VerifyEntityPermission(originalEntity, AccessRightValue.Update, entityMetadata);
            }
            var ownerObj = entityMetadata.EntityMask == EntityMaskEnum.User && entity.ContainsKey("ownerid") ? (OwnerObject)entity["ownerid"] : null;
            bool ownerChanged = ownerObj != null && !ownerObj.OwnerId.Equals(originalEntity.GetGuidValue("ownerid"));//是否改变了所有者
            if (ownerChanged)
            {
                entity.RemoveKeys("ownerid");
            }
            var result = true;
            //保存前发布事件
            PublishEvents(originalEntity, entity, OperationStage.PreOperation, entityMetadata, attributeMetadatas);
            try
            {
                _organizationDataProvider.BeginTransaction();
                InternalOnUpdate(originalEntity, entity, OperationStage.PreOperation, entityMetadata, attributeMetadatas);
                result = _organizationDataProvider.Update(entity);
                if (result)
                {
                    _mapUpdater.Update(entityMetadata, originalEntity);
                    _formulaUpdater.Update(entityMetadata, originalEntity);
                    if (ownerChanged)//改变了所有者
                    {
                        _dataAssigner.Assign(entityMetadata, entity, ownerObj);
                    }
                    _organizationDataProvider.CommitTransaction();
                    InternalOnUpdate(originalEntity, entity, OperationStage.PostOperation, entityMetadata, attributeMetadatas);
                }
                else
                {
                    _organizationDataProvider.RollBackTransaction();
                }
            }
            catch (Exception e)
            {
                _organizationDataProvider.RollBackTransaction();
                OnException(e);
            }
            if (result)
            {
                //保存成功后发布事件
                PublishEvents(originalEntity, entity, OperationStage.PostOperation, entityMetadata, attributeMetadatas);
            }
            return result;
        }

        public bool Update(IList<Entity> entities, bool ignorePermissions = false)
        {
            var entityName = entities.First().Name;
            var entityMetadata = GetEntityMetaData(entityName);
            var attributeMetadatas = _attributeFinder.FindByEntityId(entityMetadata.EntityId);
            foreach (var entity in entities)
            {
                VerifyUpdate(entity, entityMetadata, attributeMetadatas);
            }
            var query = new QueryExpression(entityName, _languageId);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition(entities.First().IdName, ConditionOperator.In, entities.Select(x => x.GetIdValue()).ToArray());
            var existsEntities = _organizationDataRetriever.RetrieveAll(query, ignorePermissions);
            if (!ignorePermissions)
            {
                foreach (var entity in existsEntities)
                {
                    VerifyEntityPermission(entity, AccessRightValue.Update, entityMetadata);
                }
            }
            foreach (var entity in existsEntities)
            {
                var originalEntity = existsEntities.First(x => x.GetIdValue().Equals(entity.GetIdValue()));
                //保存前发布事件
                PublishEvents(originalEntity, entity, OperationStage.PreOperation, entityMetadata, attributeMetadatas);
            }
            var result = true;
            _organizationDataProvider.BeginTransaction();
            try
            {
                foreach (var entity in entities)
                {
                    var originalEntity = existsEntities.First(x => x.GetIdValue().Equals(entity.GetIdValue()));
                    var ownerObj = entityMetadata.EntityMask == EntityMaskEnum.User && entity.ContainsKey("ownerid") ? (OwnerObject)entity["ownerid"] : null;
                    bool ownerChanged = ownerObj != null && !ownerObj.OwnerId.Equals(originalEntity.GetGuidValue("ownerid"));//是否改变了所有者
                    if (ownerChanged)
                    {
                        entity.RemoveKeys("ownerid");
                    }
                    InternalOnUpdate(originalEntity, entity, OperationStage.PreOperation, entityMetadata, attributeMetadatas);
                    result = _organizationDataProvider.Update(entity);
                    _mapUpdater.Update(entityMetadata, originalEntity);
                    _formulaUpdater.Update(entityMetadata, originalEntity);
                    if (ownerChanged)//改变了所有者
                    {
                        _dataAssigner.Assign(entityMetadata, entity, ownerObj);
                    }
                    InternalOnUpdate(originalEntity, entity, OperationStage.PostOperation, entityMetadata, attributeMetadatas);
                }
                _organizationDataProvider.CommitTransaction();
            }
            catch (Exception e)
            {
                _organizationDataProvider.RollBackTransaction();
                OnException(e);
            }
            if (result)
            {
                foreach (var entity in existsEntities)
                {
                    var originalEntity = existsEntities.First(x => x.GetIdValue().Equals(entity.GetIdValue()));
                    //保存后发布事件
                    PublishEvents(originalEntity, entity, OperationStage.PostOperation, entityMetadata, attributeMetadatas);
                }
            }
            return result;
        }

        public bool Update(Entity entity, QueryExpression query, bool ignorePermissions = false)
        {
            var entityMetadata = GetEntityMetaData(entity.Name);
            if (!ignorePermissions)
            {
                BindUserEntityPermissions(query, AccessRightValue.Read);
                VerifyEntityPermission(entity, AccessRightValue.Update, entityMetadata);
            }
            query.ColumnSet.Columns.Clear();
            query.ColumnSet.Columns.Add(entity.IdName);

            return _organizationDataProvider.Update(entity, _queryResolverFactory.Get(query), ignorePermissions);
        }

        /// <summary>
        /// 校验更新数据
        /// </summary>
        /// <param name="entity"></param>
        private void VerifyUpdate(Entity entity, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas)
        {
            if (!entity.ContainsKey(entity.IdName))
            {
                OnException(_loc["sdk_notspecified_primarykey"]);
            }
            if (!entity[entity.IdName].ToString().IsGuid() || entity.GetGuidValue(entity.IdName).Equals(Guid.Empty))
            {
                OnException(_loc["sdk_notspecified_primaryvalue"]);
            }
            _entityValidator.VerifyValues(entity, entityMetadata, attributeMetadatas, (e) =>
            {
                OnException(string.Join("\n", e));
            });
            if (attributeMetadatas.Exists(x => x.Name.IsCaseInsensitiveEqual("modifiedon")))
            {
                entity.AddIfNotContain("modifiedon", DateTime.Now);
            }
            if (attributeMetadatas.Exists(x => x.Name.IsCaseInsensitiveEqual("modifiedby")))
            {
                entity.AddIfNotContain("modifiedby", _user.SystemUserId);
            }
        }

        /// <summary>
        /// 更新记录时执行的方法
        /// 运行于事务中
        /// </summary>
        /// <param name="existsData"></param>
        /// <param name="newData"></param>
        /// <param name="stage"></param>
        /// <param name="entityMetadata"></param>
        public virtual void OnUpdate(Entity existsData, Entity newData, OperationStage stage, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas) { }

        /// <summary>
        /// 更新记录时触发的事件
        /// </summary>
        /// <param name="originData"></param>
        /// <param name="newData"></param>
        /// <param name="stage"></param>
        /// <param name="entityMetadata"></param>
        private void InternalOnUpdate(Entity originData, Entity newData, OperationStage stage, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas)
        {
            //plugin
            _entityPluginExecutor.Execute(OperationTypeEnum.Update, stage, newData, entityMetadata, attributeMetadatas);
            OnUpdate(originData, newData, stage, entityMetadata, attributeMetadatas);
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="originData"></param>
        /// <param name="newData"></param>
        /// <param name="stage"></param>
        /// <param name="entityMetadata"></param>
        /// <param name="attributeMetadatas"></param>
        private void PublishEvents(Entity originData, Entity newData, OperationStage stage, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas)
        {
            if (stage == OperationStage.PreOperation)
            {
                _eventPublisher.Publish(new EntityUpdatingEvent(originData, newData) { EntityMetadata = entityMetadata, AttributeMetadatas = attributeMetadatas });
            }
            else if (stage == OperationStage.PostOperation)
            {
                _eventPublisher.Publish(new EntityUpdatedEvent(originData, newData) { EntityMetadata = entityMetadata, AttributeMetadatas = attributeMetadatas });
            }
        }
    }
}