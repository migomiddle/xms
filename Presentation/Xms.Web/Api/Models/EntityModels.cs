using System;
using System.Collections.Generic;
using Xms.Business.SerialNumber.Domain;
using Xms.Form.Abstractions;
using Xms.Form.Abstractions.Component;
using Xms.Form.Domain;

namespace Xms.Web.Api.Models
{
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
}