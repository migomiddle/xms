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
using Xms.Plugin.Abstractions;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.RelationShip;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Data;
using Xms.Sdk.Event;
using Xms.Sdk.Event.AggRoot;

namespace Xms.Sdk.Client.AggRoot
{
    /// <summary>
    /// 聚合更新服务
    /// </summary>
    public class AggUpdater : DataProviderBase, IAggUpdater
    {
        private readonly IOrganizationDataProvider _organizationDataProvider;
        private readonly IDataCreater _dataCreater;
        private readonly IDataUpdater _dataUpdater;
        private readonly IDataDeleter _dataDeleter;
        private readonly IRelationShipFinder _relationShipFinder;
        private readonly IPluginExecutor<AggregateRoot, AggregateRootMetaData> _pluginExecutor;
        private readonly IAttributeFinder _attributeFinder;

        public AggUpdater(
            IAppContext appContext
            , IEntityFinder entityFinder
            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService
            , IPrincipalObjectAccessService principalObjectAccessService
            , IEventPublisher eventPublisher
            , IBusinessUnitService businessUnitService

            , IAttributeFinder attributeFinder
            , IRelationShipFinder relationShipFinder
            , IDataCreater dataCreater
            , IDataUpdater dataUpdater
            , IDataDeleter dataDeleter
            , IOrganizationDataProvider organizationDataProvider
            , IPluginExecutor<AggregateRoot, AggregateRootMetaData> pluginExecutor
            )
            : base(appContext, entityFinder, roleObjectAccessEntityPermissionService, principalObjectAccessService, eventPublisher, businessUnitService)
        {
            _attributeFinder = attributeFinder;
            _relationShipFinder = relationShipFinder;
            _organizationDataProvider = organizationDataProvider;
            _pluginExecutor = pluginExecutor;
            _dataCreater = dataCreater;
            _dataUpdater = dataUpdater;
            _dataDeleter = dataDeleter;
        }

        private AggregateRootMetaData GetAggregateRootMetaData(AggregateRoot aggregateRoot, Guid? systemFormId)
        {
            var aggRootMetaData = new AggregateRootMetaData();
            var entityMetadata = GetEntityMetaData(aggregateRoot.MainEntity.Name);
            var attributeMetadatas = _attributeFinder.FindByEntityId(entityMetadata.EntityId);
            var eam = new EntityAttributeMetadata { EntityMetadata = entityMetadata, AttributeMetadatas = attributeMetadatas };
            aggRootMetaData.MainMetadata = eam;
            aggRootMetaData.SystemFormId = systemFormId;
            aggRootMetaData.ListMetadatas = new Dictionary<string, EntityAttributeMetadata>();
            foreach (var refEntity in aggregateRoot.ChildEntities)
            {
                entityMetadata = GetEntityMetaData(refEntity.Name);
                attributeMetadatas = _attributeFinder.FindByEntityId(entityMetadata.EntityId);
                eam = new EntityAttributeMetadata { EntityMetadata = entityMetadata, AttributeMetadatas = attributeMetadatas };
                if (aggRootMetaData.ListMetadatas.ContainsKey(refEntity.Name))
                {
                    aggRootMetaData.ListMetadatas.Add(refEntity.Name, eam);
                }
            }
            return aggRootMetaData;
        }

        public bool Update(AggregateRoot aggregateRoot, Guid? systemFormId, bool ignorePermissions = false)
        {
            AggregateRootMetaData aggRootMetaData = GetAggregateRootMetaData(aggregateRoot, systemFormId);
            var result = false;
            var originalAggroot = new AggregateRoot();
            try
            {
                _organizationDataProvider.BeginTransaction();
                InternalOnUpdate(originalAggroot, aggregateRoot, OperationStage.PreOperation, aggRootMetaData);

                var thisId = aggregateRoot.MainEntity.GetIdValue();
                foreach (var c in aggregateRoot.ChildEntities)
                {
                    string name = c.Name, relationshipname = c.Relationshipname, refname = string.Empty;
                    if (relationshipname.IsNotEmpty())
                    {
                        var relationShipMetas = _relationShipFinder.FindByName(c.Relationshipname);
                        if (null != relationShipMetas && relationShipMetas.ReferencedEntityId == c.Entityid)
                        {
                            refname = relationShipMetas.ReferencingAttributeName;
                        }
                        if (refname.IsNotEmpty())
                        {
                            c.Entity.SetAttributeValue(refname, new EntityReference(aggregateRoot.MainEntity.Name, thisId));
                        }
                    }
                }
                var childEntities = aggregateRoot.ChildEntities.Where(x => x.Entitystatus != OperationTypeEnum.Delete).Select(x => x.Entity).ToList();
                var entityNames = childEntities.Select(n => n.Name).Distinct().ToList();
                foreach (var item in entityNames)
                {
                    var items = childEntities.Where(n => n.Name.IsCaseInsensitiveEqual(item)).ToList();
                    var creatingRecords = items.Where(n => n.Name.IsCaseInsensitiveEqual(item) && n.GetIdValue().Equals(Guid.Empty)).ToList();
                    if (creatingRecords.NotEmpty())
                    {
                        _dataCreater.CreateMany(creatingRecords);
                    }
                    foreach (var updItem in items.Where(n => n.Name.IsCaseInsensitiveEqual(item) && !n.GetIdValue().Equals(Guid.Empty)))
                    {
                        _dataUpdater.Update(updItem);
                    }
                }
                var childEntitiesDelete = aggregateRoot.ChildEntities.Where(x => x.Entitystatus == OperationTypeEnum.Delete).Select(x => x.Entity).ToList();
                var entityNamesDelete = childEntitiesDelete.Select(n => n.Name).Distinct().ToList();
                foreach (var item in entityNamesDelete)
                {
                    var items = childEntitiesDelete.Where(n => n.Name.IsCaseInsensitiveEqual(item)).ToList();
                    foreach (var deleteItem in items)
                    {
                        _dataDeleter.Delete(deleteItem);
                    }
                }

                result = _dataUpdater.Update(aggregateRoot.MainEntity);
                InternalOnUpdate(originalAggroot, aggregateRoot, OperationStage.PostOperation, aggRootMetaData);
                _organizationDataProvider.CommitTransaction();
            }
            catch (Exception e)
            {
                _organizationDataProvider.RollBackTransaction();
                OnException(e);
            }
            return result;
        }

        public virtual void OnUpdate(AggregateRoot existsData, AggregateRoot newData, OperationStage stage, AggregateRootMetaData AggRootMetaDatas)
        {
        }

        private void InternalOnUpdate(AggregateRoot originData, AggregateRoot newData, OperationStage stage, AggregateRootMetaData aggRootMetaDatas)
        {
            //plugin
            var entityId = aggRootMetaDatas.MainMetadata.EntityMetadata.EntityId;
            Guid? businessObjectId = aggRootMetaDatas.SystemFormId;
            PlugInType pluginType = PlugInType.Form;
            _pluginExecutor.Execute(entityId, businessObjectId, pluginType, OperationTypeEnum.Update, stage, newData, aggRootMetaDatas);
            if (stage == OperationStage.PreOperation)
            {
                _eventPublisher.Publish(new AggRootUpdatingEvent(originData, newData) { AggRootMetaData = aggRootMetaDatas });
            }
            else if (stage == OperationStage.PostOperation)
            {
                _eventPublisher.Publish(new AggRootUpdatedEvent(originData, newData) { AggRootMetaData = aggRootMetaDatas });
            }
            OnUpdate(originData, newData, stage, aggRootMetaDatas);
        }
    }
}