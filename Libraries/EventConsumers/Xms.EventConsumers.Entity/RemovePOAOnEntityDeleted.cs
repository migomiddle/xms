using Xms.Authorization.Abstractions;
using Xms.Event.Abstractions;
using Xms.Sdk.Event;

namespace Xms.EventConsumers.Entity
{
    /// <summary>
    /// 实体删除后删除相关的数据授权
    /// </summary>
    public class RemovePOAOnEntityDeleted : IConsumer<EntityDeletedEvent>
    {
        private readonly IPrincipalObjectAccessService _principalObjectAccessService;

        public RemovePOAOnEntityDeleted(IPrincipalObjectAccessService principalObjectAccessService)
        {
            _principalObjectAccessService = principalObjectAccessService;
        }

        public void HandleEvent(EntityDeletedEvent eventMessage)
        {
            _principalObjectAccessService.DeleteByObjectId(eventMessage.EntityMetadata.EntityId, eventMessage.Object.GetIdValue());
        }
    }
}