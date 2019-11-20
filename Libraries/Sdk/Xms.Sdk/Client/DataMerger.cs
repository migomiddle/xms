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
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Schema.RelationShip;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Data;
using Xms.Sdk.Event;
using Xms.Sdk.Extensions;
using Xms.Sdk.Query;

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 数据合并服务
    /// </summary>
    public class DataMerger : DataProviderBase, IDataMerger
    {
        private readonly IOrganizationDataProvider _organizationDataProvider;
        private readonly IEntityPluginExecutor _entityPluginExecutor;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IRelationShipFinder _relationShipFinder;
        private readonly IDataFinder _dataFinder;
        private readonly IQueryResolverFactory _queryResolverFactory;

        //private readonly IDataUpdater _dataUpdater;
        public DataMerger(
            IAppContext appContext
            , IEntityFinder entityFinder
            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService
            , IPrincipalObjectAccessService principalObjectAccessService
            , IEventPublisher eventPublisher
            , IBusinessUnitService businessUnitService
            , IOrganizationDataProvider organizationDataProvider
            , IEntityPluginExecutor entityPluginExecutor
            , IAttributeFinder attributeFinder
            , IRelationShipFinder relationShipFinder
            , IDataFinder dataFinder
            //, IDataUpdater dataUpdater
            , IQueryResolverFactory queryResolverFactory
            )
            : base(appContext, entityFinder, roleObjectAccessEntityPermissionService, principalObjectAccessService, eventPublisher, businessUnitService)
        {
            _organizationDataProvider = organizationDataProvider;
            _entityPluginExecutor = entityPluginExecutor;
            _attributeFinder = attributeFinder;
            _relationShipFinder = relationShipFinder;
            _dataFinder = dataFinder;
            //_dataUpdater = dataUpdater;
            _queryResolverFactory = queryResolverFactory;
        }

        public bool Merge(Guid entityId, Guid mainRecordId, Guid mergedRecordId, Dictionary<string, Guid> attributeMaps, bool ignorePermissions = false)
        {
            var entityMetadata = _entityFinder.FindById(entityId);
            //retrive main record
            var queryMain = new QueryExpression(entityMetadata.Name, _languageId);
            queryMain.ColumnSet.AllColumns = true;
            queryMain.Criteria.AddCondition(entityMetadata.Name + "id", ConditionOperator.Equal, mainRecordId);
            var mainRecord = _dataFinder.Retrieve(queryMain, ignorePermissions);
            if (mainRecord.IsEmpty())
            {
                return OnException(_loc["notfound_record"]);
            }
            //retrive merged record
            var queryMerged = new QueryExpression(entityMetadata.Name, _languageId);
            queryMerged.ColumnSet.AllColumns = true;
            queryMerged.Criteria.AddCondition(entityMetadata.Name + "id", ConditionOperator.Equal, mergedRecordId);
            var mergedRecord = _dataFinder.Retrieve(queryMerged, ignorePermissions);
            if (mergedRecord.IsEmpty())
            {
                return OnException(_loc["notfound_record"]);
            }
            var attributeMetadatas = _attributeFinder.FindByEntityId(entityId);
            var updatedRecord = new Entity(entityMetadata.Name);
            //set target record values
            foreach (var attr in attributeMetadatas)
            {
                if (attr.IsSystemControl())
                {
                    continue;
                }

                if (attributeMaps.ContainsKey(attr.Name))
                {
                    var eid = attributeMaps[attr.Name];
                    updatedRecord.SetAttributeValue(attr.Name, eid.Equals(mainRecordId) ? updatedRecord.WrapAttributeValue(_entityFinder, attr, mainRecord[attr.Name]) : updatedRecord.WrapAttributeValue(_entityFinder, attr, mergedRecord[attr.Name]));
                }
                else
                {
                    updatedRecord.SetAttributeValue(attr.Name, updatedRecord.WrapAttributeValue(_entityFinder, attr, mainRecord[attr.Name]));
                }
            }
            //this.VerifyUpdate(updatedRecord);
            updatedRecord.AddIfNotContain("modifiedon", DateTime.Now);
            updatedRecord.AddIfNotContain("modifiedby", _user.SystemUserId);
            var result = false;
            try
            {
                _organizationDataProvider.BeginTransaction();
                InternalOnMerge(mergedRecord, updatedRecord, OperationStage.PreOperation, entityMetadata, attributeMetadatas);
                //result = _dataUpdater.Update(updatedRecord, ignorePermissions);
                result = _organizationDataProvider.Update(updatedRecord);
                //update referencing records
                var relationships = _relationShipFinder.QueryByEntityId(null, entityId);
                foreach (var rs in relationships)
                {
                    var relatedEntity = new Entity(rs.ReferencingEntityName);
                    relatedEntity.SetAttributeValue(rs.ReferencingAttributeName, mainRecordId);
                    var queryRelated = new QueryExpression(rs.ReferencingEntityName, _languageId);
                    queryRelated.ColumnSet.AddColumns(rs.ReferencingEntityName + "id");
                    queryRelated.Criteria.AddCondition(rs.ReferencingAttributeName, ConditionOperator.Equal, mergedRecordId);
                    _organizationDataProvider.Update(relatedEntity, _queryResolverFactory.Get(queryRelated), ignorePermissions: true);
                }
                //disabled original record
                var disabledEntity = new Entity(entityMetadata.Name);
                disabledEntity.SetIdValue(mergedRecordId);
                disabledEntity.SetAttributeValue("statecode", false);
                //result = _dataUpdater.Update(disabledEntity, ignorePermissions);
                result = _organizationDataProvider.Update(disabledEntity);
                if (result)
                {
                    _organizationDataProvider.CommitTransaction();
                    InternalOnMerge(mergedRecord, updatedRecord, OperationStage.PostOperation, entityMetadata, attributeMetadatas);
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

        /// <summary>
        /// 记录合并时执行的方法
        /// 运行于事务中
        /// </summary>
        /// <param name="merged"></param>
        /// <param name="target"></param>
        /// <param name="stage"></param>
        /// <param name="entityMetadata"></param>
        public virtual void OnMerge(Entity merged, Entity target, OperationStage stage, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas) { }

        /// <summary>
        /// 合并记录时触发的事件
        /// </summary>
        /// <param name="merged"></param>
        /// <param name="target"></param>
        /// <param name="stage"></param>
        /// <param name="entityMetadata"></param>
        private void InternalOnMerge(Entity merged, Entity target, OperationStage stage, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas)
        {
            //plugin
            _entityPluginExecutor.Execute(OperationTypeEnum.Merge, stage, target, entityMetadata, attributeMetadatas);
            if (stage == OperationStage.PreOperation)
            {
                _eventPublisher.Publish(new EntityMergingEvent() { Merged = merged, Target = target, EntityMetadata = entityMetadata });
            }
            else if (stage == OperationStage.PostOperation)
            {
                _eventPublisher.Publish(new EntityMergedEvent() { Merged = merged, Target = target, EntityMetadata = entityMetadata });
            }
            OnMerge(merged, target, stage, entityMetadata, attributeMetadatas);
        }
    }
}