using System;
using System.Collections.Generic;
using Xms.Flow.Domain;

namespace Xms.Flow
{
    public interface IWorkFlowHandlerFinder
    {
        List<Guid> GetCurrentHandlerId(WorkFlowInstance instance, WorkFlowProcess prevStep, int handlerIdType, string handlers);
    }
}