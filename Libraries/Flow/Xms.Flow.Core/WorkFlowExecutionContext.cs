using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Xms.Flow.Abstractions;
using Xms.Flow.Domain;
using Xms.Schema.Domain;

namespace Xms.Flow.Core
{
    public sealed class WorkFlowExecutionContext
    {
        public WorkFlowInstance InstanceInfo { get; set; }
        public WorkFlowProcess ProcessInfo { get; set; }

        public WorkFlowProcessState ProcessState { get; set; }
        public string Description { get; set; }

        public Entity EntityMetaData { get; set; }
        public int Attachments { get; set; }
        public List<IFormFile> AttachmentFiles { get; set; }
    }
}