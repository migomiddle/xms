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
    /// 聚合创建服务
    /// </summary>
    public class AggCreater : DataProviderBase, IAggCreater
    {
        private readonly IOrganizationDataProvider _organizationDataProvider;
        private readonly IDataCreater _dataCreater;
        private readonly IPluginExecutor<AggregateRoot, AggregateRootMetaData> _pluginExecutor;

        private readonly IAttributeFinder _attributeFinder;
        private readonly IRelationShipFinder _relationShipFinder;

        public AggCreater(
            IAppContext appContext
            , IEntityFinder entityFinder
            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService
            , IPrincipalObjectAccessService principalObjectAccessService
            , IEventPublisher eventPublisher
            , IBusinessUnitService businessUnitService

            , IAttributeFinder attributeFinder
            , IRelationShipFinder relationShipFinder
            , IDataCreater dataCreater
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

        public Guid Create(AggregateRoot aggregateRoot, Guid objectId, Guid? systemFormId = null, bool ignorePermissions = false)
        {
            AggregateRootMetaData aggRootMetaData = GetAggregateRootMetaData(aggregateRoot, systemFormId);
            var thisId = Guid.Empty;
            try
            {
                _organizationDataProvider.BeginTransaction();

                InternalOnCreate(aggregateRoot, OperationStage.PreOperation, aggRootMetaData);

                //关联Id
                thisId = objectId;
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

                var childEntities = aggregateRoot.ChildEntities.Select(x => x.Entity).ToList();
                var entityNames = childEntities.Select(n => n.Name).Distinct().ToList();
                foreach (var item in entityNames)
                {
                    var items = childEntities.Where(n => n.Name.IsCaseInsensitiveEqual(item)).ToList();
                    var creatingRecords = items.Where(n => n.Name.IsCaseInsensitiveEqual(item) && n.GetIdValue().Equals(Guid.Empty)).ToList();
                    if (creatingRecords.NotEmpty())
                    {
                        _dataCreater.CreateMany(creatingRecords);
                    }
                }
                aggregateRoot.MainEntity.SetIdValue(thisId);
                _dataCreater.Create(aggregateRoot.MainEntity);

                InternalOnCreate(aggregateRoot, OperationStage.PostOperation, aggRootMetaData);

                _organizationDataProvider.CommitTransaction();
            }
            catch (Exception e)
            {
                _organizationDataProvider.RollBackTransaction();
                OnException(e);
            }
            return thisId;
        }

        public virtual void OnCreate(AggregateRoot data, OperationStage stage, AggregateRootMetaData aggRootMetaData)
        {
        }

        /// <summary>
        /// 创建记录时触发的事件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="stage"></param>
        /// <param name="entityMetadata"></param>
        private void InternalOnCreate(AggregateRoot data, OperationStage stage, AggregateRootMetaData aggRootMetaDatas)
        {
            //plugin
            var entityId = aggRootMetaDatas.MainMetadata.EntityMetadata.EntityId;
            Guid? businessObjectId = aggRootMetaDatas.SystemFormId;
            PlugInType pluginType = PlugInType.Form;
            _pluginExecutor.Execute(entityId, businessObjectId, pluginType, OperationTypeEnum.Update, stage, data, aggRootMetaDatas);
            if (stage == OperationStage.PreOperation)
            {
                //event publishing
                _eventPublisher.Publish(new AggRootCreatingEvent(data) { AggRootMetaData = aggRootMetaDatas });
            }
            else if (stage == OperationStage.PostOperation)
            {
                //event publishing
                _eventPublisher.Publish(new AggRootCreatedEvent(data) { AggRootMetaData = aggRootMetaDatas });
            }
            OnCreate(data, stage, aggRootMetaDatas);
        }
    }
}