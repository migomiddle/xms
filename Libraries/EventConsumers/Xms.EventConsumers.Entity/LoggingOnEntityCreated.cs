using Xms.Event.Abstractions;
using Xms.Logging.DataLog;
using Xms.Sdk.Event;

namespace Xms.EventConsumers.Entity
{
    /// <summary>
    /// 实体记录创建后执行日志记录
    /// </summary>
    public class LoggingOnEntityCreated : IConsumer<EntityCreatedEvent>
    {
        private readonly IEntityLogService _entityLogService;

        public LoggingOnEntityCreated(IEntityLogService entityLogService)
        {
            _entityLogService = entityLogService;
        }

        public void HandleEvent(EntityCreatedEvent eventMessage)
        {
            if (eventMessage.EntityMetadata.LogEnabled)
            {
                _entityLogService.CreatedLog(eventMessage.Object, eventMessage.EntityMetadata, eventMessage.AttributeMetadatas);
            }
        }
    }
}