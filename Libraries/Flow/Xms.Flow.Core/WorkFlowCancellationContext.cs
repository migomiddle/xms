using System;
using Xms.Schema.Domain;

namespace Xms.Flow.Core
{
    public class WorkFlowCancellationContext
    {
        public Guid ObjectId { get; set; }
        public Entity EntityMetaData { get; set; }
    }
}