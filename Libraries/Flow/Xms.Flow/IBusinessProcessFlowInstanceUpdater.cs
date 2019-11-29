using System;
using Xms.Core.Context;
using Xms.Flow.Domain;

namespace Xms.Flow
{
    public interface IBusinessProcessFlowInstanceUpdater
    {
        bool Update(Func<UpdateContext<BusinessProcessFlowInstance>, UpdateContext<BusinessProcessFlowInstance>> context);

        bool Update(BusinessProcessFlowInstance entity);

        bool UpdateBack(Guid workFlowId, Guid instanceId, Guid processStageId, Guid recordId);

        bool UpdateForward(Guid workflowId, Guid instanceId, Guid processStageId, Guid recordId);

        void UpdateOnRecordDeleted(Guid entityId, Guid recordId);
    }
}