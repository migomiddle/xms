using Xms.Event.Abstractions;
using Xms.Logging.DataLog;
using Xms.Sdk.Event;

namespace Xms.EventConsumers.Entity
{
    /// <summary>
    /// 实体共享时执行日志记录
    /// </summary>
    public class LoggingOnEntityShared : IConsumer<EntitySharedEvent>
    {
        private readonly IEntityLogService _entityLogService;

        public LoggingOnEntityShared(IEntityLogService entityLogService)
        {
            _entityLogService = entityLogService;
        }

        public void HandleEvent(EntitySharedEvent eventMessage)
        {
            if (eventMessage.EntityMetadata.LogEnabled)
            {
                _entityLogService.SharedLog(eventMessage.Data, eventMessage.EntityMetadata);
            }
        }
    }
}