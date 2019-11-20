using Xms.Event.Abstractions;
using Xms.Logging.DataLog;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Event;

namespace Xms.EventConsumers.Entity
{
    /// <summary>
    /// 实体分派时执行日志记录
    /// </summary>
    public class LoggingOnEntityAssigned : IConsumer<EntityAssignedEvent>
    {
        private readonly IEntityLogService _entityLogService;

        public LoggingOnEntityAssigned(IEntityLogService entityLogService)
        {
            _entityLogService = entityLogService;
        }

        public void HandleEvent(EntityAssignedEvent eventMessage)
        {
            if (eventMessage.EntityMetadata.LogEnabled)
            {
                _entityLogService.AssignedLog(eventMessage.Data, eventMessage.OriginData["ownerid"] as OwnerObject, eventMessage.OriginData["ownerid"] as OwnerObject, eventMessage.EntityMetadata);
            }
        }
    }
}