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
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Data;
using Xms.Sdk.Event;
using Xms.Security.Domain;

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 数据共享服务
    /// </summary>
    public class DataSharer : DataProviderBase, IDataSharer
    {
        private readonly IOrganizationDataProvider _organizationDataProvider;
        private readonly IEntityPluginExecutor _entityPluginExecutor;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDataFinder _dataFinder;

        public DataSharer(
            IAppContext appContext
            , IEntityFinder entityFinder
            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService
            , IPrincipalObjectAccessService principalObjectAccessService
            , IEventPublisher eventPublisher
            , IBusinessUnitService businessUnitService
            , IOrganizationDataProvider organizationDataProvider
            , IEntityPluginExecutor entityPluginExecutor
            , IAttributeFinder attributeFinder
            , IDataFinder dataFinder)
            : base(appContext, entityFinder, roleObjectAccessEntityPermissionService, principalObjectAccessService, eventPublisher, businessUnitService)
        {
            _organizationDataProvider = organizationDataProvider;
            _entityPluginExecutor = entityPluginExecutor;
            _attributeFinder = attributeFinder;
            _dataFinder = dataFinder;
        }

        public bool Share(Guid entityId, Guid recordId, List<PrincipalObjectAccess> principals, bool ignorePermissions = false)
        {
            try
            {
                var entityMetadata = _entityFinder.FindById(entityId);
                var query = new QueryExpression(entityMetadata.Name, _languageId);
                query.ColumnSet.AllColumns = true;
                query.Criteria.AddCondition(entityMetadata.Name + "id", ConditionOperator.Equal, recordId);
                var entity = _dataFinder.Retrieve(query);
                if (!ignorePermissions)
                {
                    VerifyEntityPermission(entity, AccessRightValue.Share, entityMetadata);
                }
                var attributeMetadatas = _attributeFinder.FindByEntityId(entityMetadata.EntityId);
                _organizationDataProvider.BeginTransaction();
                InternalOnShare(entity, OperationStage.PreOperation, entityMetadata, attributeMetadatas);
                _principalObjectAccessService.DeleteByObjectId(entityId, recordId);
                if (principals.NotEmpty())
                {
                    foreach (var item in principals)
                    {
                        item.PrincipalObjectAccessId = Guid.NewGuid();
                    }
                    _principalObjectAccessService.CreateMany(principals);
                }
                _organizationDataProvider.CommitTransaction();
                InternalOnShare(entity, OperationStage.PostOperation, entityMetadata, attributeMetadatas);
                return true;
            }
            catch (Exception e)
            {
                _organizationDataProvider.RollBackTransaction();
                return OnException(e);
            }
        }

        /// <summary>
        /// 共享记录时执行的方法
        /// 运行于事务中
        /// </summary>
        /// <param name="data"></param>
        /// <param name="stage"></param>
        /// <param name="entityMetadata"></param>
        public virtual void OnShare(Entity data, OperationStage stage, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas) { }

        /// <summary>
        /// 共享记录时触发的事件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="stage"></param>
        /// <param name="entityMetadata"></param>
        private void InternalOnShare(Entity data, OperationStage stage, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas)
        {
            //plugin
            _entityPluginExecutor.Execute(OperationTypeEnum.Share, stage, data, entityMetadata, attributeMetadatas);
            if (stage == OperationStage.PreOperation)
            {
                _eventPublisher.Publish(new EntitySharingEvent() { Data = data, EntityMetadata = entityMetadata, Principals = null });
            }
            else if (stage == OperationStage.PostOperation)
            {
                _eventPublisher.Publish(new EntitySharedEvent() { Data = data, EntityMetadata = entityMetadata, Principals = null });
            }
            OnShare(data, stage, entityMetadata, attributeMetadatas);
        }
    }
}