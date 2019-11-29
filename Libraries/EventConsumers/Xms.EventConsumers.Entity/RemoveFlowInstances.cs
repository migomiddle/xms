using Xms.Event.Abstractions;
using Xms.Flow;
using Xms.Sdk.Event;

namespace Xms.EventConsumers.Entity
{
    /// <summary>
    /// 删除流程关联实例
    /// </summary>
    public class RemoveFlowInstances : IConsumer<EntityDeletedEvent>
    {
        private readonly IWorkFlowInstanceService _WorkFlowInstanceService;
        private readonly IBusinessProcessFlowInstanceService _businessProcessFlowInstanceService;
        private readonly IBusinessProcessFlowInstanceUpdater _businessProcessFlowInstanceUpdater;

        public RemoveFlowInstances(IWorkFlowInstanceService WorkFlowInstanceService
            , IBusinessProcessFlowInstanceService businessProcessFlowInstanceService
            , IBusinessProcessFlowInstanceUpdater businessProcessFlowInstanceUpdater)
        {
            _WorkFlowInstanceService = WorkFlowInstanceService;
            _businessProcessFlowInstanceService = businessProcessFlowInstanceService;
            _businessProcessFlowInstanceUpdater = businessProcessFlowInstanceUpdater;
        }

        public void HandleEvent(EntityDeletedEvent eventMessage)
        {
            if (eventMessage.EntityMetadata.WorkFlowEnabled)
            {
                _WorkFlowInstanceService.DeleteByObjectId(eventMessage.EntityMetadata.EntityId, eventMessage.Object.GetIdValue());
            }
            if (eventMessage.EntityMetadata.BusinessFlowEnabled)
            {
                _businessProcessFlowInstanceService.DeleteById(eventMessage.EntityMetadata.EntityId);
                //update business stage
                _businessProcessFlowInstanceUpdater.UpdateOnRecordDeleted(eventMessage.EntityMetadata.EntityId, eventMessage.Object.GetIdValue());
            }
        }
    }
}