using System;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Web.Api.Models
{
    public class CreateRecordModel
    {
        public Guid? EntityId { get; set; }

        public string EntityName { get; set; }

        public string Data { get; set; }
    }

    public class CreateFromMapModel
    {
        public Guid? SourceEntityId { get; set; }
        public string SourceEntityName { get; set; }
        public Guid? TargetEntityId { get; set; }
        public string TargetEntityName { get; set; }
        public Guid SourceRecordId { get; set; }
    }

    public class DataUpdateModel
    {
        public Guid? EntityId { get; set; }

        public string EntityName { get; set; }

        public string Data { get; set; }
    }

    public class SaveDataModel
    {
        public Guid? RecordId { get; set; }
        public Guid EntityId { get; set; }
        public Guid? FormId { get; set; }
        public string RelationShipName { get; set; }
        public Guid? ReferencedRecordId { get; set; }
        public Guid StageId { get; set; }
        public Guid BusinessFlowId { get; set; }
        public Guid BusinessFlowInstanceId { get; set; }
        public string Data { get; set; }
        public string Child { get; set; }
    }

    public class SaveChildDataModel
    {
        public string EntityName { get; set; }
        public string Child { get; set; }
        public Guid ParentId { get; set; }
    }

    public class DeleteEntityRecordModel
    {
        public Guid EntityId { get; set; }

        public string EntityName { get; set; }

        public Guid[] RecordId { get; set; }
    }

    public class RetrieveMultipleModel
    {
        public QueryExpression Query { get; set; }
        public bool IsAll { get; set; } = false;
    }

    public class RetrieveByIdModel
    {
        public string EntityName { get; set; }
        public Guid Id { get; set; }
    }

    public class RetrieveReferencedRecordModel
    {
        public Guid EntityId { get; set; }
        public string Value { get; set; }
        public bool AllColumns { get; set; } = false;
    }

    public class AssignedModel
    {
        public Guid EntityId { get; set; }
        public Guid[] ObjectId { get; set; }

        public Guid OwnerId { get; set; }

        public int OwnerIdType { get; set; }
    }

    public class AggregateModel
    {
        public Guid QueryViewId { get; set; }

        public FilterExpression Filter { get; set; }
    }
}