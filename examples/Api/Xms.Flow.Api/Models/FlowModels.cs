using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Xms.Flow.Abstractions;
using Xms.Flow.Domain;

namespace Xms.Flow.Api.Models
{
    public class StartWorkFlowModel
    {
        public Guid EntityId { get; set; }
        public Guid RecordId { get; set; }
        public string Description { get; set; }
        public List<WorkFlow> WorkFlows { get; set; }
        public Guid WorkflowId { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
    public class UpdateProcessStageModel
    {
        public Guid EntityId { get; set; }
        public Guid RecordId { get; set; }
        public Guid FromRecordId { get; set; }
        public Guid WorkflowId { get; set; }
        public Guid StageId { get; set; }
        public Guid? InstanceId { get; set; }
    }
    public class WorkFlowProcessedModel
    {
        public Guid WorkFlowProcessId { get; set; }

        public string Description { get; set; }

        public WorkFlowProcessState ProcessState { get; set; }

        public List<IFormFile> Attachments { get; set; }
    }
}
