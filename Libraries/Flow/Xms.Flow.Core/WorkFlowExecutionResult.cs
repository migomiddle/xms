using System;
using System.Collections.Generic;
using Xms.Flow.Domain;

namespace Xms.Flow.Core
{
    public sealed class WorkFlowExecutionResult
    {
        public List<Guid> NextHandlerId { get; set; }

        public WorkFlowInstance Instance { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}