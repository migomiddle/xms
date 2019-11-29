using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Xms.Business.SerialNumber.Domain;
using Xms.Core;
using Xms.Data.Import.Domain;
using Xms.DataMapping.Domain;
using Xms.Flow.Abstractions;
using Xms.Flow.Domain;
using Xms.Form.Abstractions;
using Xms.Form.Abstractions.Component;
using Xms.Form.Domain;
using Xms.Logging.DataLog.Domain;
using Xms.QueryView.Abstractions.Component;
using Xms.Schema.Domain;
using Xms.Sdk.Abstractions.Query;
using Xms.Security.Domain;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Models
{
    public class DataListModel : BasePaged<dynamic>
    {
        public Guid? EntityId { get; set; }
        public Guid? QueryViewId { get; set; }
        public string EntityName { get; set; }
        public string RelationShipName { get; set; }
        public Guid? TargetFormId { get; set; }
        public string Theme { get; set; }

        public bool IsShowButtons { get; set; } = true;

        public bool IsEnabledFilter { get; set; } = true;

        public bool IsEnabledFastSearch { get; set; } = true;

        public bool IsEnabledViewSelector { get; set; } = true;
        public bool PagingEnabled { get; set; } = true;

        public bool OnlyData { get; set; } = false;
        public bool IsShowChart { get; set; } = true;
        public int DefaultEmptyRows { get; set; } = 5;

        public bool IncludeIndex { get; set; }

        public string ExportTitle { get; set; }

        public int ExportType { get; set; }
        public string Q { get; set; }
        public string QField { get; set; }
        public FilterExpression Filter { get; set; }

        public string GridId { get; set; }
        public bool IsEditable { get; set; }
        public Guid? ReferencedRecordId { get; set; }
    }

    public class EntityGridModel : DataListModel
    {
        public GridDescriptor Grid { get; set; }
        public QueryView.Domain.QueryView QueryView { get; set; }
        public List<QueryView.Domain.QueryView> QueryViews { get; set; }
        public Schema.Domain.RelationShip RelationShipMeta { get; set; }
        public List<Schema.Domain.Entity> EntityList { get; set; }
        public List<Schema.Domain.Attribute> AttributeList { get; set; }
        public List<Schema.Domain.RelationShip> RelationShipList { get; set; }

        public List<RibbonButton.Domain.RibbonButton> RibbonButtons { get; set; }

        public List<Schema.Domain.Attribute> NonePermissionFields { get; set; }

        public dynamic AggregationData { get; set; }
        public List<AggregateExpressionField> AggregateFields { get; set; }
    }

    public class KanbanGridModel : BasePaged<dynamic>
    {
        public Guid? EntityId { get; set; }
        public Guid? QueryId { get; set; }
        public string EntityName { get; set; }
        public string AggregateField { get; set; }
        public string GroupField { get; set; }
        public QueryView.Domain.QueryView QueryView { get; set; }
        public FilterExpression Filter { get; set; }

        public List<Schema.Domain.Entity> EntityList { get; set; }
        public List<Schema.Domain.Attribute> AttributeList { get; set; }
        public List<Schema.Domain.RelationShip> RelationShipList { get; set; }

        public List<dynamic> GroupingDatas { get; set; }
        private int _groupingTop = 5;

        public int GroupingTop
        {
            get { return _groupingTop; }
            set { _groupingTop = value; }
        }

        public AggregateType AggType { get; set; }
    }

    public class SelectEntityRecordsDialogModel : BasePaged<dynamic>
    {
        public Guid? EntityId { get; set; }
        public Guid? QueryId { get; set; }
        public string EntityName { get; set; }
        public string RelationShipName { get; set; }
        public Schema.Domain.RelationShip RelationShipMeta { get; set; }
        public Guid? ReferencedRecordId { get; set; }

        public string Q { get; set; }
        public string QField { get; set; }
        public GridDescriptor Grid { get; set; }
        public QueryView.Domain.QueryView QueryView { get; set; }
        public List<QueryView.Domain.QueryView> QueryViews { get; set; }

        public FilterExpression Filter { get; set; }

        public List<Schema.Domain.Entity> EntityList { get; set; }
        public List<Schema.Domain.Attribute> AttributeList { get; set; }
        public List<Schema.Domain.RelationShip> RelationShipList { get; set; }

        public List<Schema.Domain.Attribute> NonePermissionFields { get; set; }
        public bool OnlyEnabledRecords { get; set; } = true;
    }

    public class EntityFormModel
    {
        public Guid EntityId { get; set; }
        public string EntityName { get; set; }
        public Guid? RecordId { get; set; }
        public Guid? CopyId { get; set; }
        public string CopyRelationShipName { get; set; }
        public Guid? FormId { get; set; }

        public bool ReadOnly { get; set; }
        public Guid StageId { get; set; }
        public Guid BusinessFlowId { get; set; }
        public Guid BusinessFlowInstanceId { get; set; }
        public string RelationShipName { get; set; }
        public Guid? ReferencedRecordId { get; set; }
    }

    public class EditRecordModel
    {
        public Guid? RecordId { get; set; }
        public Guid? EntityId { get; set; }
        public Guid? FormId { get; set; }
        public bool ReadOnly { get; set; }

        public FormDescriptor Form { get; set; }

        public SystemForm FormInfo { get; set; }

        public Schema.Domain.Entity EntityMetaData { get; set; }

        public Core.Data.Entity Entity { get; set; }
        public List<Schema.Domain.Attribute> AttributeList { get; set; }
        public List<RibbonButton.Domain.RibbonButton> RibbonButtons { get; set; }
        public string RelationShipName { get; set; }
        public Guid? ReferencedRecordId { get; set; }

        public SerialNumberRule SnRule { get; set; }
        public FormState FormState { get; set; }
        public bool HasBasePermission { get; set; }
        public int WorkFlowProcessState { get; set; }
        public string AttributeChanged { get; set; }
        public Guid StageId { get; set; }
        public Guid BusinessFlowId { get; set; }
        public Guid BusinessFlowInstanceId { get; set; }
    }

    public class SetEntityRecordStateModel
    {
        public Guid EntityId { get; set; }

        public Guid[] RecordId { get; set; }

        public RecordState State { get; set; }

        public bool ReloadPage { get; set; } = true;
    }

    public class ChildEntityModel
    {
        public string Name { get; set; }

        public List<Entity> Datas { get; set; }
    }

    public class ImportModel
    {
        public Guid EntityId { get; set; }
        public string EntityName { get; set; }

        public List<Schema.Domain.Attribute> Attributes { get; set; }

        public Dictionary<string, Schema.Domain.Attribute> MapData { get; set; }

        public IFormFile DataFile { get; set; }

        public string DataFileName { get; set; }

        public string MapCustomizations { get; set; }

        public string Name { get; set; }
        public int DuplicateDetection { get; set; }
        public Guid ImportMapId { get; set; }
        public Guid ImportFileId { get; set; }
        public List<ImportMap> ImportMaps { get; set; }
    }

    public class EntityLogsModel : BasePaged<EntityLog>
    {
        public Guid EntityId { get; set; }
        public OperationTypeEnum? OperationType { get; set; }
        public List<Schema.Domain.Attribute> Attributes { get; set; }
    }

    public class StartWorkFlowModel
    {
        public Guid EntityId { get; set; }
        public Guid RecordId { get; set; }
        public string Description { get; set; }
        public List<WorkFlow> WorkFlows { get; set; }
        public Guid WorkflowId { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }

    #region flowing

    public class WorkFlowProcessingModel
    {
        public Guid EntityId { get; set; }
        public Guid RecordId { get; set; }
        public WorkFlowInstance InstanceInfo { get; set; }
        public WorkFlowProcess ProcessInfo { get; set; }
        public List<WorkFlowProcess> ProcessList { get; set; }
    }

    public class WorkFlowProcessedModel
    {
        public Guid WorkFlowProcessId { get; set; }

        public string Description { get; set; }

        public WorkFlowProcessState ProcessState { get; set; }

        public List<IFormFile> Attachments { get; set; }
    }

    public class WorkFlowInstanceModel : BasePaged<WorkFlowInstance>
    {
        public Guid WorkFlowId { get; set; }
    }

    public class WorkFlowInstanceDetailModel : BasePaged<WorkFlowInstance>
    {
        public WorkFlow FlowInfo { get; set; }
        public List<WorkFlowStep> Steps { get; set; }
    }

    public class WorkFlowProcessModel : BasePaged<WorkFlowProcess>
    {
        public Guid WorkFlowInstanceId { get; set; }
        public WorkFlowInstance InstanceInfo { get; set; }
        public WorkFlow FlowInfo { get; set; }
    }

    public class WorkFlowStateListModel : BasePaged<dynamic>
    {
        public int StateCode { get; set; }
        public Guid? EntityId { get; set; }
        public long HandledCount { get; set; }
        public long HandlingCount { get; set; }
        public long ApplyHandledCount { get; set; }
        public long ApplyHandlingCount { get; set; }
    }

    public class BusinessProcessArgsModel
    {
        public Guid EntityId { get; set; }
        public Guid RecordId { get; set; }
        public Guid? BusinessflowId { get; set; }
        public Guid? BusinessflowInstanceId { get; set; }
    }

    public class BusinessProcessModel
    {
        public Guid EntityId { get; set; }
        public Guid RecordId { get; set; }
        public Core.Data.Entity Data { get; set; }
        public Guid CurrentStageId { get; set; }
        public WorkFlow BusinessFlow { get; set; }
        public BusinessProcessFlowInstance BusinessFlowInstance { get; set; }
        public List<ProcessStage> Stages { get; set; }
        public List<Schema.Domain.Attribute> Attributes { get; set; }
        public Dictionary<string, object> Steps { get; set; }
        public List<Schema.Domain.RelationShip> RelationShips { get; set; }
        public Dictionary<string, object> RelatedRecords { get; set; }
    }

    #endregion flowing

    #region principle

    public class SharedModel
    {
        public Guid EntityId { get; set; }
        public Guid ObjectId { get; set; }
        public int TargetType { get; set; }
        public List<Guid> TargetId { get; set; }
        public string PrincipalsJson { get; set; }
        public List<PrincipalObjectAccess> Principals { get; set; }
    }

    public class SharedPrincipalsModel : BasePaged<PrincipalObjectAccess>
    {
        public Guid EntityId { get; set; }
        public Guid ObjectId { get; set; }
    }

    public class AssignModel
    {
        public Guid EntityId { get; set; }
        public Guid[] ObjectId { get; set; }

        public Guid OwnerId { get; set; }

        public int OwnerIdType { get; set; }

        public Schema.Domain.Entity EntityMetaData { get; set; }
    }

    public class AssignUserAllRecordsModel
    {
        public Guid UserId { get; set; }
    }

    public class AppendRecordModel
    {
        public Guid EntityId { get; set; }
        public Guid ObjectId { get; set; }

        public List<EntityMap> EntityMaps { get; set; }

        public List<Schema.Domain.Entity> TargetEntityMetas { get; set; }
    }

    public class MergeModel
    {
        public Guid EntityId { get; set; }
        public Guid RecordId1 { get; set; }
        public Guid RecordId2 { get; set; }
        public Guid MainRecordId { get; set; }

        public Core.Data.Entity Record1 { get; set; }
        public Core.Data.Entity Record2 { get; set; }
        public Schema.Domain.Entity EntityMetas { get; set; }
        public List<Schema.Domain.Attribute> Attributes { get; set; }
    }

    #endregion principle
}