using Xms.Event.Abstractions;
using Xms.Logging.DataLog;
using Xms.Sdk.Event;

namespace Xms.EventConsumers.Entity
{
    /// <summary>
    /// 实体更新时执行日志记录
    /// </summary>
    public class LoggingOnEntityUpdated : IConsumer<EntityUpdatedEvent>
    {
        private readonly IEntityLogService _entityLogService;

        public LoggingOnEntityUpdated(IEntityLogService entityLogService)
        {
            _entityLogService = entityLogService;
        }

        public void HandleEvent(EntityUpdatedEvent eventMessage)
        {
            if (eventMessage.EntityMetadata.LogEnabled)
            {
                _entityLogService.UpdatedLog(eventMessage.Origin, eventMessage.Updated, eventMessage.EntityMetadata, eventMessage.AttributeMetadatas);
            }
        }
    }
}