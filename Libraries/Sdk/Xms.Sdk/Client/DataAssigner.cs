using System;
using System.Collections.Generic;
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
using Xms.Schema.RelationShip;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Data;
using Xms.Sdk.Event;
using Xms.Sdk.Extensions;
using Xms.Sdk.Query;

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 数据分派服务
    /// </summary>
    public class DataAssigner : DataProviderBase, IDataAssigner
    {
        private readonly IOrganizationDataProvider _organizationDataProvider;
        private readonly IOrganizationDataRetriever _organizationDataRetriever;
        private readonly IQueryResolverFactory _queryResolverFactory;
        private readonly IEntityPluginExecutor _entityPluginExecutor;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IRelationShipFinder _relationShipFinder;

        public DataAssigner(
            IAppContext appContext
            , IEntityFinder entityFinder
            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService
            , IPrincipalObjectAccessService principalObjectAccessService
            , IEventPublisher eventPublisher
            , IBusinessUnitService businessUnitService
            , IEntityPluginExecutor entityPluginExecutor
            , IAttributeFinder attributeFinder
            , IRelationShipFinder relationShipFinder
            , IOrganizationDataProvider organizationDataProvider
            , IOrganizationDataRetriever organizationDataRetriever
            , IQueryResolverFactory queryResolverFactory
            )
            : base(appContext, entityFinder, roleObjectAccessEntityPermissionService, principalObjectAccessService, eventPublisher, businessUnitService)
        {
            _organizationDataProvider = organizationDataProvider;
            _organizationDataRetriever = organizationDataRetriever;
            _queryResolverFactory = queryResolverFactory;
            _entityPluginExecutor = entityPluginExecutor;
            _attributeFinder = attributeFinder;
            _relationShipFinder = relationShipFinder;
        }

        public bool Assign(Schema.Domain.Entity entityMetadata, Entity entity, OwnerObject owner, bool ignorePermissions = false)
        {
            if (!ignorePermissions)
            {
                VerifyEntityPermission(entity, AccessRightValue.Assign, entityMetadata);
            }
            var recordId = entity.GetIdValue();
            Entity ownerEntity = null;
            Entity updateEntity = new Entity(entityMetadata.Name);
            updateEntity.SetIdValue(recordId);
            updateEntity.SetAttributeValue("ownerid", owner);
            if (owner.OwnerType == OwnerTypes.SystemUser)
            {
                if (owner.OwnerId.Equals(_user.SystemUserId))
                {
                    updateEntity.SetAttributeValue("owningbusinessunit", _user.BusinessUnitId);
                }
                else
                {
                    //business unit
                    var queryBusinessUnit = new QueryExpression("systemuser", _languageId);
                    queryBusinessUnit.ColumnSet.AddColumn("businessunitid");
                    queryBusinessUnit.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, owner.OwnerId);
                    ownerEntity = _organizationDataRetriever.Retrieve(queryBusinessUnit, true);

                    updateEntity.SetAttributeValue("owningbusinessunit", ownerEntity.GetGuidValue("businessunitid"));
                }
                updateEntity.SetAttributeValue("modifiedon", DateTime.Now);
                updateEntity.SetAttributeValue("modifiedby", _user.SystemUserId);
            }
            else
            {
                updateEntity.SetAttributeValue("owningbusinessunit", null);
            }
            var attributeMetadatas = _attributeFinder.FindByEntityId(entityMetadata.EntityId);
            var result = true;
            try
            {
                _organizationDataProvider.BeginTransaction();
                InternalOnAssign(entity, updateEntity, OperationStage.PreOperation, entityMetadata, attributeMetadatas);
                result = _organizationDataProvider.Update(updateEntity);
                if (result)
                {
                    //assign cascade relationship, 1: N
                    var relationships = _relationShipFinder.Query(n => n
                        .Where(f => f.ReferencedEntityId == entityMetadata.EntityId && f.CascadeAssign != (int)CascadeUpdateType.None)
                    );
                    if (relationships.NotEmpty())
                    {
                        foreach (var rs in relationships)
                        {
                            var relatedEntityMeta = _entityFinder.FindById(rs.ReferencingEntityId);
                            if (relatedEntityMeta.EntityMask == EntityMaskEnum.Organization)
                            {
                                continue;
                            }

                            var queryRelated = new QueryExpression(rs.ReferencingEntityName, _languageId);
                            queryRelated.ColumnSet.AddColumns(rs.ReferencingEntityName + "id");
                            queryRelated.Criteria.AddCondition(rs.ReferencingAttributeName, ConditionOperator.Equal, recordId);
                            //update related records
                            Entity updEntity = new Entity(rs.ReferencingEntityName);
                            updEntity.SetAttributeValue("ownerid", owner);
                            if (owner.OwnerType == OwnerTypes.SystemUser)
                            {
                                updEntity.SetAttributeValue("owningbusinessunit", ownerEntity.GetGuidValue("businessunitid"));
                            }
                            else
                            {
                                updEntity.SetAttributeValue("owningbusinessunit", null);
                            }
                            updEntity.SetAttributeValue("modifiedon", DateTime.Now);
                            updEntity.SetAttributeValue("modifiedby", _user.SystemUserId);
                            _organizationDataProvider.Update(updEntity, _queryResolverFactory.Get(queryRelated), true);
                        }
                    }
                    _organizationDataProvider.CommitTransaction();
                    InternalOnAssign(entity, updateEntity, OperationStage.PostOperation, entityMetadata, attributeMetadatas);
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
            return true;
        }

        public bool Assign(Guid entityId, Guid recordId, OwnerObject owner, bool ignorePermissions = false)
        {
            var entityMetadata = _entityFinder.FindById(entityId);
            var query = new QueryExpression(entityMetadata.Name, _languageId);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition(entityMetadata.Name + "id", ConditionOperator.Equal, recordId);
            var entity = _organizationDataRetriever.Retrieve(query, ignorePermissions);

            return Assign(entityMetadata, entity, owner, ignorePermissions);
        }

        /// <summary>
        /// 分派记录时执行的方法
        /// 运行于事务中
        /// </summary>
        /// <param name="data"></param>
        /// <param name="newOwnerEntity"></param>
        /// <param name="stage"></param>
        /// <param name="entityMetadata"></param>
        public virtual void OnAssign(Entity data, Entity newOwnerEntity, OperationStage stage, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas) { }

        /// <summary>
        /// 分派记录时触发的事件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="newOwnerEntity"></param>
        /// <param name="stage"></param>
        /// <param name="entityMetadata"></param>
        private void InternalOnAssign(Entity data, Entity newOwnerEntity, OperationStage stage, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas)
        {
            //plugin
            _entityPluginExecutor.Execute(OperationTypeEnum.Assign, stage, newOwnerEntity, entityMetadata, attributeMetadatas);
            if (stage == OperationStage.PreOperation)
            {
                _eventPublisher.Publish(new EntityAssigningEvent() { OriginData = data, Data = newOwnerEntity, EntityMetadata = entityMetadata });
            }
            else if (stage == OperationStage.PostOperation)
            {
                _eventPublisher.Publish(new EntityAssignedEvent() { OriginData = data, Data = newOwnerEntity, EntityMetadata = entityMetadata });
            }
            OnAssign(data, newOwnerEntity, stage, entityMetadata, attributeMetadatas);
        }
    }
}