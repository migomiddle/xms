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
using Xms.Schema.Extensions;
using Xms.Schema.RelationShip;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Data;
using Xms.Sdk.Event;

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 数据删除服务
    /// </summary>
    public class DataDeleter : DataProviderBase, IDataDeleter
    {
        private readonly IOrganizationDataProvider _organizationDataProvider;
        private readonly IOrganizationDataRetriever _organizationDataRetriever;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IRelationShipFinder _relationShipFinder;
        private readonly IEntityPluginExecutor _entityPluginExecutor;
        private readonly IMapUpdater _mapUpdater;
        private readonly IFormulaUpdater _formulaUpdater;
        private readonly IAggregateService _aggregateService;

        public DataDeleter(
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
            , IRelationShipFinder relationShipFinder
            , IMapUpdater mapUpdater
            , IFormulaUpdater formulaUpdater
            , IAggregateService aggregateService
            )
            : base(appContext, entityFinder, roleObjectAccessEntityPermissionService, principalObjectAccessService, eventPublisher, businessUnitService)
        {
            _organizationDataProvider = organizationDataProvider;
            _organizationDataRetriever = organizationDataRetriever;
            _attributeFinder = attributeFinder;
            _entityPluginExecutor = entityPluginExecutor;
            _relationShipFinder = relationShipFinder;
            _mapUpdater = mapUpdater;
            _formulaUpdater = formulaUpdater;
            _aggregateService = aggregateService;
        }

        private bool DeleteCore(Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas, Entity record, bool ignorePermissions = false)
        {
            if (!ignorePermissions)
            {
                VerifyEntityPermission(record, AccessRightValue.Delete, entityMetadata);
            }
            //cascade relationship, 1: N
            var relationships = _relationShipFinder.Query(n => n
                .Where(f => f.ReferencedEntityId == entityMetadata.EntityId)
            );
            //check referenced
            var cascadeDeleteRestrict = relationships.Where(n => n.ReferencedEntityId == entityMetadata.EntityId && n.CascadeDelete == (int)CascadeDeleteType.Restrict && n.RelationshipType == RelationShipType.ManyToOne);

            var primaryAttr = attributeMetadatas.Find(n => n.TypeIsPrimaryKey());
            var primarykey = primaryAttr.Name;

            record.IdName = primarykey;
            var recordId = record.GetIdValue();
            foreach (var cdr in cascadeDeleteRestrict)
            {
                var referencingRecord = _aggregateService.Count(cdr.ReferencingEntityName, new FilterExpression(LogicalOperator.And).AddCondition(cdr.ReferencingAttributeName, ConditionOperator.Equal, recordId));
                if (referencingRecord > 0)
                {
                    _relationShipFinder.WrapLocalizedLabel(cdr);
                    return OnException(_loc["referenced"] + ": " + cdr.ReferencingEntityLocalizedName);
                }
            }
            var result = false;
            try
            {
                _organizationDataProvider.BeginTransaction();
                InternalOnDelete(record, OperationStage.PreOperation, entityMetadata, attributeMetadatas);
                //delete related records
                var cascadeDelete = relationships.Where(n => n.ReferencedEntityId == entityMetadata.EntityId && n.CascadeDelete == (int)CascadeDeleteType.All && n.RelationshipType == RelationShipType.ManyToOne).ToList();
                if (cascadeDelete.NotEmpty())
                {
                    DeleteRelatedRecords(entityMetadata, attributeMetadatas, cascadeDelete, recordId);
                }
                //delete main record
                result = _organizationDataProvider.Delete(record.Name, recordId, primarykey);
                if (result)
                {
                    //update maps
                    _mapUpdater.Update(entityMetadata, record, true);
                    _formulaUpdater.Update(entityMetadata, record);
                    _organizationDataProvider.CommitTransaction();
                    InternalOnDelete(record, OperationStage.PostOperation, entityMetadata, attributeMetadatas);
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

            return result;
        }

        private bool DeleteRelatedRecords(Schema.Domain.Entity entityMetaData, List<Schema.Domain.Attribute> attributeMetadatas, List<Schema.Domain.RelationShip> relationships, Guid parentId)
        {
            var result = true;
            var primaryKeyField = attributeMetadatas.Find(x => x.TypeIsPrimaryKey());
            foreach (var rs in relationships)
            {
                //delete the 'N' records
                var query = new QueryExpression(rs.ReferencingEntityName, _languageId);
                query.ColumnSet.AddColumn(primaryKeyField.Name);
                query.Criteria.AddCondition(rs.ReferencingAttributeName, ConditionOperator.Equal, parentId);
                var datas = _organizationDataRetriever.RetrieveAll(query, true)?.ToList();
                if (datas.NotEmpty())
                {
                    var entityMetadata2 = GetEntityMetaData(datas.First().Name);
                    var relationships2 = _relationShipFinder.Query(n => n
                        .Where(f => f.ReferencedEntityId == entityMetadata2.EntityId && f.RelationshipType == RelationShipType.ManyToOne && f.CascadeDelete == (int)CascadeDeleteType.All)
                    );
                    foreach (var item in datas)
                    {
                        if (relationships2.NotEmpty())
                        {
                            DeleteRelatedRecords(entityMetadata2, attributeMetadatas, relationships2, item.GetIdValue());
                        }
                        InternalOnDelete(item, OperationStage.PreOperation, entityMetaData, attributeMetadatas);
                        result = _organizationDataProvider.Delete(item.Name, item.GetIdValue());
                        if (result)
                        {
                            InternalOnDelete(item, OperationStage.PostOperation, entityMetaData, attributeMetadatas);
                            _mapUpdater.Update(entityMetadata2, item, true);
                            _formulaUpdater.Update(entityMetadata2, item);
                        }
                    }
                }
            }
            return result;
        }

        public bool Delete(Entity record, bool ignorePermissions = false)
        {
            var entityMetadata = GetEntityMetaData(record.Name);
            var attributeMetadatas = _attributeFinder.FindByEntityId(entityMetadata.EntityId);
            return this.DeleteCore(entityMetadata, attributeMetadatas, record, ignorePermissions);
        }

        public bool Delete(string name, Guid id, bool ignorePermissions = false)
        {
            var entityMetadata = GetEntityMetaData(name);
            var attributeMetadatas = _attributeFinder.FindByEntityId(entityMetadata.EntityId);
            var primaryAttr = attributeMetadatas.Find(n => n.TypeIsPrimaryKey());
            var primarykey = primaryAttr.Name;
            var query = new QueryExpression(name, _languageId);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition(primarykey, ConditionOperator.Equal, id);
            var record = _organizationDataRetriever.Retrieve(query, true);
            return this.DeleteCore(entityMetadata, attributeMetadatas, record, ignorePermissions);
        }

        public bool Delete(string name, IEnumerable<Guid> ids, bool ignorePermissions = false)
        {
            var isSuccess = false;
            foreach (var id in ids)
            {
                isSuccess = this.Delete(name, id, ignorePermissions);
            }
            return isSuccess;
        }

        /// <summary>
        /// 删除记录时执行的方法
        /// 运行于事务中
        /// </summary>
        /// <param name="data"></param>
        /// <param name="stage"></param>
        /// <param name="entityMetadata"></param>
        public virtual void OnDelete(Entity data, OperationStage stage, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas) { }

        /// <summary>
        /// 删除记录时触发的事件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="stage"></param>
        /// <param name="entityMetadata"></param>
        private void InternalOnDelete(Entity data, OperationStage stage, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas)
        {
            //plugin
            _entityPluginExecutor.Execute(OperationTypeEnum.Delete, stage, data, entityMetadata, attributeMetadatas);
            if (stage == OperationStage.PreOperation)
            {
                _eventPublisher.Publish(new EntityDeletingEvent(data) { EntityMetadata = entityMetadata, AttributeMetadatas = attributeMetadatas });
            }
            else if (stage == OperationStage.PostOperation)
            {
                _eventPublisher.Publish(new EntityDeletedEvent(data) { EntityMetadata = entityMetadata, AttributeMetadatas = attributeMetadatas });
            }
            OnDelete(data, stage, entityMetadata, attributeMetadatas);
        }
    }
}