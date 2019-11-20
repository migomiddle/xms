using Xms.Event.Abstractions;
using Xms.Logging.DataLog;
using Xms.Sdk.Event;

namespace Xms.EventConsumers.Entity
{
    /// <summary>
    /// 实体删除时执行日志记录
    /// </summary>
    public class LoggingOnEntityDeleted : IConsumer<EntityDeletedEvent>
    {
        private readonly IEntityLogService _entityLogService;

        public LoggingOnEntityDeleted(IEntityLogService entityLogService)
        {
            _entityLogService = entityLogService;
        }

        public void HandleEvent(EntityDeletedEvent eventMessage)
        {
            if (eventMessage.EntityMetadata.LogEnabled)
            {
                _entityLogService.DeletedLog(eventMessage.Object, eventMessage.EntityMetadata, eventMessage.AttributeMetadatas);
            }
        }
    }
}