using System;

namespace Xms.Web.Api.Models
{
    public class UpdateProcessStageModel
    {
        public Guid EntityId { get; set; }
        public Guid RecordId { get; set; }
        public Guid FromRecordId { get; set; }
        public Guid WorkflowId { get; set; }
        public Guid StageId { get; set; }
        public Guid? InstanceId { get; set; }
    }
}