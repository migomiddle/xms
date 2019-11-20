using PetaPoco;
using System;
using Xms.Core;
using Xms.Flow.Abstractions;

namespace Xms.Flow.Domain
{
    [TableName("WorkFlowProcess")]
    [PrimaryKey("WorkFlowProcessId", AutoIncrement = false)]
    public class WorkFlowProcess
    {
        public Guid WorkFlowProcessId { get; set; } = Guid.NewGuid();
        public Guid WorkFlowInstanceId { get; set; }
        public string Name { get; set; }
        public int StepOrder { get; set; }
        public WorkFlowProcessState StateCode { get; set; }
        public DateTime? HandleTime { get; set; }
        public int Attachments { get; set; }
        public string UniqueCode { get; set; }
        public string AuthAttributes { get; set; }
        public DateTime? StartTime { get; set; }
        public int ReturnType { get; set; }
        public int ReturnTo { get; set; }
        public bool AllowAssign { get; set; }
        public bool AllowCancel { get; set; }
        public int HandlerIdType { get; set; }
        public string Handlers { get; set; }
        public string Description { get; set; }

        public Guid FormId { get; set; }
        public bool AttachmentRequired { get; set; }
        public string AttachmentExts { get; set; }
        public string NodeName { get; set; }
        public Guid HandlerId { get; set; }

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "HandlerId", LinkToTableName = "SystemUser", LinkToFieldName = "SystemUserId", TargetFieldName = "Name")]
        public string HandlerIdName { get; set; }
    }
}