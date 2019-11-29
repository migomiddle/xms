using System;
using Xms.Flow.Domain;
using Xms.Schema.Domain;

namespace Xms.Flow.Core.Events
{
    public class WorkFlowCancelledEvent
    {
        public Guid ObjectId { get; set; }
        public Entity EntityMetaData { get; set; }
        public WorkFlowInstance Instance { get; set; }
        public WorkFlowProcess CurrentStep { get; set; }
        public WorkFlowCancellationResult Result { get; set; }
    }
}