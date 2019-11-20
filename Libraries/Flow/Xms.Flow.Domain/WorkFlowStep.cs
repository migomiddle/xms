using PetaPoco;
using System;

namespace Xms.Flow.Domain
{
    [TableName("WorkFlowStep")]
    [PrimaryKey("WorkFlowStepId", AutoIncrement = false)]
    public class WorkFlowStep
    {
        public Guid WorkFlowStepId { get; set; } = Guid.NewGuid();
        public Guid WorkFlowId { get; set; }
        public string Name { get; set; }
        public int StepOrder { get; set; }
        public string AuthAttributes { get; set; }
        public bool AllowAssign { get; set; }
        public bool AllowCancel { get; set; }
        public int ReturnType { get; set; }
        public int ReturnTo { get; set; }
        public int HandlerIdType { get; set; }
        public string Handlers { get; set; }
        public string Conditions { get; set; }
        public string Description { get; set; }
        public Guid FormId { get; set; }
        public string Style { get; set; }
        public int NodeType { get; set; }
        public string NodeName { get; set; }
        public bool AttachmentRequired { get; set; }
        public string AttachmentExts { get; set; }
    }
}