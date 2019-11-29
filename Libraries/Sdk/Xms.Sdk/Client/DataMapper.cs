using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core;
using Xms.Core.Data;
using Xms.DataMapping;
using Xms.DataMapping.Abstractions;
using Xms.DataMapping.Domain;
using Xms.Event.Abstractions;
using Xms.Infrastructure;
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

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 单据记录转换服务
    /// </summary>
    public class DataMapper : DataProviderBase, IDataMapper
    {
        private readonly IOrganizationDataProvider _organizationDataProvider;
        private readonly IEntityPluginExecutor _entityPluginExecutor;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IRelationShipFinder _relationShipFinder;
        private readonly IEntityMapFinder _entityMapFinder;
        private readonly IAttributeMapFinder _attributeMapFinder;
        private readonly IDataFinder _dataFinder;
        private readonly IDataCreater _dataCreater;

        public DataMapper(
            IAppContext appContext
            , IEntityFinder entityFinder
            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService
            , IPrincipalObjectAccessService principalObjectAccessService
            , IEventPublisher eventPublisher
            , IBusinessUnitService businessUnitService
            , IOrganizationDataProvider organizationDataProvider
            , IEntityMapFinder entityMapFinder
            , IAttributeMapFinder attributeMapFinder
            , IRelationShipFinder relationShipFinder
            , IEntityPluginExecutor entityPluginExecutor
            , IAttributeFinder attributeFinder
            , IDataFinder dataFinder
            , IDataCreater dataCreater)
            : base(appContext, entityFinder, roleObjectAccessEntityPermissionService, principalObjectAccessService, eventPublisher, businessUnitService)
        {
            _organizationDataProvider = organizationDataProvider;
            _entityMapFinder = entityMapFinder;
            _attributeMapFinder = attributeMapFinder;
            _relationShipFinder = relationShipFinder;
            _entityPluginExecutor = entityPluginExecutor;
            _attributeFinder = attributeFinder;
            _dataFinder = dataFinder;
            _dataCreater = dataCreater;
        }

        public Guid Create(Guid sourceEntityId, Guid targetEntityId, Guid sourceRecordId, bool ignorePermissions = false)
        {
            var entityMap = _entityMapFinder.Find(sourceEntityId, targetEntityId);
            if (entityMap == null || entityMap.StateCode == RecordState.Disabled)
            {
                OnException(_loc["entitymap_isempty"]);
                return Guid.Empty;
            }
            var entity = new Entity(entityMap.SourceEnttiyName);
            entity.SetAttributeValue("ownerid", _user.SystemUserId);
            entity.SetAttributeValue("owningbusinessunit", _user.BusinessUnitId);
            //检查追加权限
            var sourceEntityMetas = _entityFinder.FindById(entityMap.SourceEntityId);
            VerifyEntityPermission(entity, AccessRightValue.Append, sourceEntityMetas);
            //检查被追加权限
            var targetEntityMetas = _entityFinder.FindById(entityMap.TargetEntityId);
            VerifyEntityPermission(entity, AccessRightValue.AppendTo, targetEntityMetas);
            //转换类型
            if (entityMap.MapType == MapType.CopyOne)
            {
                return CreateFromMap_Copy(entityMap, sourceRecordId);
            }
            else if (entityMap.MapType == MapType.CopyMany)
            {
                return CreateFromMap_Copy(entityMap, sourceRecordId);
            }
            else if (entityMap.MapType == MapType.Control)
            {
                return CreateFromMap_Control(entityMap, sourceRecordId);
            }
            else if (entityMap.MapType == MapType.ForceControl)
            {
                return CreateFromMap_Control(entityMap, sourceRecordId);
            }
            return Guid.Empty;
        }

        private Guid CreateFromMap_Copy(EntityMap entityMap, Guid sourceRecordId)
        {
            var headTargetEntityMeta = _entityFinder.FindById(entityMap.TargetEntityId);
            var headTargetAttributes = _attributeFinder.FindByEntityId(entityMap.TargetEntityId);
            //引用源实体的字段
            var headRelationShipMeta = _relationShipFinder.FindByName(entityMap.RelationShipName);
            var refAttr = headTargetAttributes.Find(n => n.AttributeId == headRelationShipMeta.ReferencingAttributeId);
            if (entityMap.MapType == MapType.CopyOne)
            {
                //查询是否已生成记录
                QueryExpression query_target = new QueryExpression(headTargetEntityMeta.Name, _languageId);
                query_target.ColumnSet.AddColumn(headTargetAttributes.Find(n => n.TypeIsPrimaryKey()).Name);
                query_target.Criteria.AddCondition(refAttr.Name, ConditionOperator.Equal, sourceRecordId);
                var existsRecord = _dataFinder.Retrieve(query_target);
                if (existsRecord.NotEmpty())
                {
                    OnException(_loc["entitymap_copyone_error"]);
                    return Guid.Empty;
                }
            }
            Guid newId = Guid.Empty;
            //源记录
            var sourceRecord = _dataFinder.RetrieveById(entityMap.SourceEnttiyName, sourceRecordId);
            if (sourceRecord.IsEmpty())
            {
                OnException(_loc["notfound_record"]);
                return Guid.Empty;
            }
            //单据头
            var attributeMaps = _attributeMapFinder.Query(n => n.Where(f => f.EntityMapId == entityMap.EntityMapId));
            if (attributeMaps.IsEmpty())
            {
                OnException(_loc["entitymap_emptyheadattributemap"]);
                return Guid.Empty;
            }
            //单据体
            var childEntityMap = _entityMapFinder.FindByParentId(entityMap.EntityMapId);
            //单据头字段元数据
            var headSourceAttributes = _attributeFinder.FindByEntityId(entityMap.SourceEntityId);
            //新增单据头信息
            Entity headEntity = new Entity(entityMap.TargetEnttiyName);
            foreach (var attrMap in attributeMaps)
            {
                if (!headSourceAttributes.Exists(n => n.AttributeId == attrMap.SourceAttributeId) || !headTargetAttributes.Exists(n => n.AttributeId == attrMap.TargetAttributeId))
                {
                    continue;
                }
                var attr = headTargetAttributes.Find(n => n.AttributeId == attrMap.TargetAttributeId);
                if (attr == null)
                {
                    continue;
                }
                var value = sourceRecord[attrMap.SourceAttributeName];
                if (value == null && attrMap.DefaultValue.IsNotEmpty())
                {
                    value = attrMap.DefaultValue;
                }
                headEntity.SetAttributeValue(attr.Name, sourceRecord.WrapAttributeValue(_entityFinder, attr, value));
            }
            //关联来源单据ID
            headEntity.SetAttributeValue(refAttr.Name, sourceRecord.WrapAttributeValue(_entityFinder, refAttr, sourceRecord.GetIdValue()));
            try
            {
                _organizationDataProvider.BeginTransaction();
                InternalOnMap(sourceRecord, headEntity, OperationStage.PreOperation, headTargetEntityMeta, headTargetAttributes);
                newId = _dataCreater.Create(headEntity);
                //新增单据体信息
                if (childEntityMap != null)
                {
                    var childAttributeMaps = _attributeMapFinder.Query(n => n.Where(f => f.EntityMapId == childEntityMap.EntityMapId));
                    if (childAttributeMaps.NotEmpty())
                    {
                        var childTargetEntityMeta = _entityFinder.FindById(childEntityMap.TargetEntityId);
                        var childTargetAttributesMeta = _attributeFinder.FindByEntityId(childEntityMap.TargetEntityId);
                        var childSourceAttributesMeta = _attributeFinder.FindByEntityId(childEntityMap.SourceEntityId);
                        var childRelationShips = childEntityMap.RelationShipName.SplitSafe(",");
                        //源单据体与源单据头的关系
                        var childSourceRelationShipMeta = _relationShipFinder.FindByName(childRelationShips[0]);
                        //目标单据体与目标单据头的关系
                        var childTargetRelationShipMeta = _relationShipFinder.FindByName(childRelationShips[1]);
                        //源单据体数据
                        QueryExpression query_source = new QueryExpression(childEntityMap.SourceEnttiyName, _languageId);
                        query_source.ColumnSet.AllColumns = true;
                        var refKey = headSourceAttributes.Find(n => n.AttributeId == childSourceRelationShipMeta.ReferencedAttributeId).Name;
                        query_source.Criteria.AddCondition(refKey, ConditionOperator.Equal, sourceRecordId);
                        var childSourceRecords = _dataFinder.RetrieveAll(query_source);
                        if (childSourceRecords.NotEmpty())
                        {
                            //引用单据头的字段
                            var headRefAttr = childTargetAttributesMeta.Find(n => n.AttributeId == childTargetRelationShipMeta.ReferencingAttributeId);
                            //引用源单据体的字段
                            var refSourceAttr = childTargetAttributesMeta.Find(n => n.ReferencedEntityId.HasValue && n.ReferencedEntityId.Value == childEntityMap.SourceEntityId);
                            foreach (var item in childSourceRecords)
                            {
                                //目标单据体
                                Entity childTargetRecord = new Entity(childEntityMap.TargetEnttiyName);
                                foreach (var attrMap in childAttributeMaps)
                                {
                                    if (!childSourceAttributesMeta.Exists(n => n.AttributeId == attrMap.SourceAttributeId) || !childTargetAttributesMeta.Exists(n => n.AttributeId == attrMap.TargetAttributeId))
                                    {
                                        continue;
                                    }
                                    var attr = childTargetAttributesMeta.Find(n => n.AttributeId == attrMap.TargetAttributeId);
                                    if (attr == null)
                                    {
                                        continue;
                                    }
                                    var value = item[attrMap.SourceAttributeName];
                                    if (value == null && attrMap.DefaultValue.IsNotEmpty())
                                    {
                                        value = attrMap.DefaultValue;
                                    }
                                    childTargetRecord.SetAttributeValue(attrMap.TargetAttributeName, sourceRecord.WrapAttributeValue(_entityFinder, attr, value));
                                }
                                //关联来源单据体记录ID
                                if (refSourceAttr != null)
                                {
                                    childTargetRecord.SetAttributeValue(refSourceAttr.Name, sourceRecord.WrapAttributeValue(_entityFinder, refSourceAttr, item.GetIdValue()));
                                }
                                //单据头ID
                                childTargetRecord.SetAttributeValue(headRefAttr.Name, sourceRecord.WrapAttributeValue(_entityFinder, headRefAttr, newId));
                                _dataCreater.Create(childTargetRecord);
                            }
                        }
                    }
                }
                _organizationDataProvider.CommitTransaction();
                InternalOnMap(sourceRecord, headEntity, OperationStage.PostOperation, headTargetEntityMeta, headTargetAttributes);
            }
            catch (Exception e)
            {
                _organizationDataProvider.RollBackTransaction();
                newId = Guid.Empty;
                OnException(e);
            }
            return newId;
        }

        private Guid CreateFromMap_Control(EntityMap entityMap, Guid sourceRecordId)
        {
            var headTargetEntityMeta = _entityFinder.FindById(entityMap.TargetEntityId);
            //单据头字段元数据
            var headTargetAttributes = _attributeFinder.FindByEntityId(entityMap.TargetEntityId);
            var headSourceAttributes = _attributeFinder.FindByEntityId(entityMap.SourceEntityId);
            //引用源实体的字段
            var headRelationShipMeta = _relationShipFinder.FindByName(entityMap.RelationShipName);
            var refAttr = headTargetAttributes.Find(n => n.AttributeId == headRelationShipMeta.ReferencingAttributeId);
            Guid newId = Guid.Empty;
            //源记录
            var sourceRecord = _dataFinder.RetrieveById(entityMap.SourceEnttiyName, sourceRecordId);
            if (sourceRecord.IsEmpty())
            {
                OnException(_loc["notfound_record"]);
                return Guid.Empty;
            }
            //单据头映射
            var attributeMaps = _attributeMapFinder.Query(n => n.Where(f => f.EntityMapId == entityMap.EntityMapId));
            if (attributeMaps.IsEmpty())
            {
                OnException(_loc["entitymap_emptyheadattributemap"]);
                return Guid.Empty;
            }
            //检查源单记录是否已关闭
            if (entityMap.MapType == MapType.ForceControl)
            {
                var headControlAttributes = attributeMaps.Where(n => !n.ClosedAttributeId.Equals(Guid.Empty)).ToList();
                if (headControlAttributes.NotEmpty())
                {
                    foreach (var ca in headControlAttributes)
                    {
                        if (sourceRecord.GetBoolValue(ca.ClosedAttributeName))
                        {
                            OnException(_loc["entitymap_source_closed"]);
                            return Guid.Empty;
                        }
                    }
                }
            }
            Entity headEntity = new Entity(entityMap.TargetEnttiyName);
            foreach (var attrMap in attributeMaps)
            {
                var attr = headTargetAttributes.Find(n => n.AttributeId == attrMap.TargetAttributeId);
                if (attr == null)
                {
                    continue;
                }
                var value = sourceRecord[attrMap.SourceAttributeName];
                if (value == null && attrMap.DefaultValue.IsNotEmpty())
                {
                    value = attrMap.DefaultValue;
                }
                headEntity.SetAttributeValue(attr.Name, headEntity.WrapAttributeValue(_entityFinder, attr, value));
            }
            if (entityMap.MapType == MapType.Control || entityMap.MapType == MapType.ForceControl)
            {
                var controlAttributes = attributeMaps.Where(n => !n.RemainAttributeId.Equals(Guid.Empty)).ToList();
                if (controlAttributes.NotEmpty())
                {
                    foreach (var ca in controlAttributes)
                    {
                        //设置目标单据钩稽字段值
                        headEntity.SetAttributeValue(ca.TargetAttributeName, sourceRecord.GetDecimalValue(ca.SourceAttributeName) - sourceRecord.GetDecimalValue(ca.RemainAttributeName));
                    }
                }
            }
            //关联来源单据ID
            headEntity.SetAttributeValue(refAttr.Name, headEntity.WrapAttributeValue(_entityFinder, refAttr, sourceRecord.GetIdValue()));
            try
            {
                _organizationDataProvider.BeginTransaction();
                InternalOnMap(sourceRecord, headEntity, OperationStage.PreOperation, headTargetEntityMeta, headTargetAttributes);
                newId = _dataCreater.Create(headEntity);
                //反填钩稽字段
                if (entityMap.MapType == MapType.Control || entityMap.MapType == MapType.ForceControl)
                {
                    var controlAttributes = attributeMaps.Where(n => !n.RemainAttributeId.Equals(Guid.Empty)).ToList();
                    if (controlAttributes.NotEmpty())
                    {
                        Entity updateSourceRecord = new Entity(sourceRecord.Name);
                        updateSourceRecord.SetIdValue(sourceRecord.GetIdValue());
                        foreach (var ca in controlAttributes)
                        {
                            var attr = _attributeFinder.FindById(ca.ClosedAttributeId);
                            updateSourceRecord.SetAttributeValue(ca.RemainAttributeName, sourceRecord.GetDecimalValue(ca.RemainAttributeName) + headEntity.GetDecimalValue(ca.TargetAttributeName));
                            updateSourceRecord.SetAttributeValue(ca.ClosedAttributeName, updateSourceRecord.WrapAttributeValue(_entityFinder, attr, sourceRecord.GetDecimalValue(ca.SourceAttributeName) - updateSourceRecord.GetDecimalValue(ca.RemainAttributeName) == 0));
                        }
                        _organizationDataProvider.Update(updateSourceRecord);
                    }
                }
                //单据体
                var childEntityMap = _entityMapFinder.FindByParentId(entityMap.EntityMapId);
                if (childEntityMap != null)
                {
                    var childAttributeMap = _attributeMapFinder.Query(n => n.Where(f => f.EntityMapId == childEntityMap.EntityMapId));
                    if (childAttributeMap.NotEmpty())
                    {
                        var childTargetEntityMeta = _entityFinder.FindById(childEntityMap.TargetEntityId);
                        var childTargetAttributesMeta = _attributeFinder.FindByEntityId(childEntityMap.TargetEntityId);
                        var childRelationShips = childEntityMap.RelationShipName.SplitSafe(",");
                        var childSourceRelationShipMeta = _relationShipFinder.FindByName(childRelationShips[0]);
                        var childTargetRelationShipMeta = _relationShipFinder.FindByName(childRelationShips[1]);
                        //源单据体数据
                        QueryExpression query = new QueryExpression(childEntityMap.SourceEnttiyName, _languageId);
                        query.ColumnSet.AllColumns = true;
                        query.Criteria.AddCondition(entityMap.SourceEnttiyName + "id", ConditionOperator.Equal, sourceRecordId);
                        var childSourceRecords = _dataFinder.RetrieveAll(query);
                        if (childSourceRecords.NotEmpty())
                        {
                            int childSuccessCount = 0;
                            //引用单据头的字段
                            var headRefAttr = childTargetAttributesMeta.Find(n => n.AttributeId == childTargetRelationShipMeta.ReferencingAttributeId);
                            //引用源单据体的字段
                            var refSourceAttr = childTargetAttributesMeta.Find(n => n.ReferencedEntityId.HasValue && n.ReferencedEntityId.Value == childEntityMap.SourceEntityId);
                            foreach (var childSourceRecord in childSourceRecords)
                            {
                                var isClosed = false;
                                //检查源单记录是否已关闭
                                if (childEntityMap.MapType == MapType.ForceControl)
                                {
                                    var controlAttributes = childAttributeMap.Where(n => !n.RemainAttributeId.Equals(Guid.Empty)).ToList();
                                    if (controlAttributes.NotEmpty())
                                    {
                                        foreach (var ca in controlAttributes)
                                        {
                                            if (childSourceRecord.GetBoolValue(ca.ClosedAttributeName))
                                            {
                                                isClosed = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (isClosed)
                                {
                                    continue;
                                }
                                //目标单据体
                                Entity childTargetRecord = new Entity(childEntityMap.TargetEnttiyName);
                                foreach (var attrMap in childAttributeMap)
                                {
                                    var attr = childTargetAttributesMeta.Find(n => n.AttributeId == attrMap.TargetAttributeId);
                                    if (attr == null)
                                    {
                                        continue;
                                    }
                                    var value = childTargetRecord[attrMap.SourceAttributeName];
                                    if (value == null && attrMap.DefaultValue.IsNotEmpty())
                                    {
                                        value = attrMap.DefaultValue;
                                    }
                                    childTargetRecord.SetAttributeValue(attrMap.TargetAttributeName, childTargetRecord.WrapAttributeValue(_entityFinder, attr, value));
                                }
                                //关联来源单据体记录ID
                                if (refSourceAttr != null)
                                {
                                    childTargetRecord.SetAttributeValue(refSourceAttr.Name, childTargetRecord.WrapAttributeValue(_entityFinder, refSourceAttr, childSourceRecord.GetIdValue()));
                                }
                                //单据头ID
                                childTargetRecord.SetAttributeValue(headRefAttr.Name, childTargetRecord.WrapAttributeValue(_entityFinder, headRefAttr, newId));
                                if (childEntityMap.MapType == MapType.Control || childEntityMap.MapType == MapType.ForceControl)
                                {
                                    var controlAttributes = childAttributeMap.Where(n => !n.RemainAttributeId.Equals(Guid.Empty)).ToList();
                                    if (controlAttributes.NotEmpty())
                                    {
                                        foreach (var ca in controlAttributes)
                                        {
                                            //设置目标单据钩稽字段值
                                            childTargetRecord.SetAttributeValue(ca.TargetAttributeName, childSourceRecord.GetDecimalValue(ca.SourceAttributeName) - childSourceRecord.GetDecimalValue(ca.RemainAttributeName));
                                        }
                                    }
                                }
                                _dataCreater.Create(childTargetRecord);
                                //反填钩稽字段
                                if (childEntityMap.MapType == MapType.Control || childEntityMap.MapType == MapType.ForceControl)
                                {
                                    var controlAttributes = childAttributeMap.Where(n => !n.RemainAttributeId.Equals(Guid.Empty)).ToList();
                                    if (controlAttributes.NotEmpty())
                                    {
                                        Entity updateSourceRecord = new Entity(childSourceRecord.Name);
                                        updateSourceRecord.SetIdValue(childSourceRecord.GetIdValue());
                                        foreach (var ca in controlAttributes)
                                        {
                                            var attr = _attributeFinder.FindById(ca.ClosedAttributeId);
                                            if (attr == null)
                                            {
                                                continue;
                                            }
                                            updateSourceRecord.SetAttributeValue(ca.RemainAttributeName, childSourceRecord.GetDecimalValue(ca.RemainAttributeName) + childTargetRecord.GetDecimalValue(ca.TargetAttributeName));
                                            updateSourceRecord.SetAttributeValue(ca.ClosedAttributeName, updateSourceRecord.WrapAttributeValue(_entityFinder, attr, childSourceRecord.GetDecimalValue(ca.SourceAttributeName) - updateSourceRecord.GetDecimalValue(ca.RemainAttributeName) == 0));
                                        }
                                        _organizationDataProvider.Update(updateSourceRecord);
                                    }
                                }
                                childSuccessCount++;
                            }
                            //强制钩稽时校验
                            if (childSuccessCount == 0 && childEntityMap.MapType == MapType.ForceControl)
                            {
                                //RollBackTransaction();
                                throw new XmsException(_loc["entitymap_source_closed"]);
                            }
                        }
                    }
                }
                _organizationDataProvider.CommitTransaction();
                InternalOnMap(sourceRecord, headEntity, OperationStage.PostOperation, headTargetEntityMeta, headTargetAttributes);
            }
            catch (Exception e)
            {
                _organizationDataProvider.RollBackTransaction();
                newId = Guid.Empty;
                OnException(e);
            }
            return newId;
        }

        /// <summary>
        /// 单据转换时触发的事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="stage"></param>
        /// <param name="targetEntityMeta"></param>
        private void InternalOnMap(Entity source, Entity target, OperationStage stage, Schema.Domain.Entity targetEntityMeta, List<Schema.Domain.Attribute> targetAttributeMetadatas)
        {
            //plugin
            _entityPluginExecutor.Execute(OperationTypeEnum.Merge, stage, target, targetEntityMeta, targetAttributeMetadatas);
            if (stage == OperationStage.PreOperation)
            {
                _eventPublisher.Publish(new EntityMappingEvent() { Source = source, Target = target, EntityMetadata = targetEntityMeta });
            }
            else if (stage == OperationStage.PostOperation)
            {
                _eventPublisher.Publish(new EntityMappedEvent() { Source = source, Target = target, EntityMetadata = targetEntityMeta });
            }
            OnMap(source, target, stage, targetEntityMeta, targetAttributeMetadatas);
        }

        /// <summary>
        /// 单据转换时执行的方法
        /// 运行于事务中
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="stage"></param>
        /// <param name="entityMetadata"></param>
        public virtual void OnMap(Entity source, Entity target, OperationStage stage, Schema.Domain.Entity targetEntityMeta, List<Schema.Domain.Attribute> targetAttributeMetadatas) { }
    }
}