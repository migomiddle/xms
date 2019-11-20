using Xms.Event.Abstractions;
using Xms.Logging.DataLog;
using Xms.Sdk.Event;

namespace Xms.EventConsumers.Entity
{
    /// <summary>
    /// 实体合并时执行日志记录
    /// </summary>
    public class LoggingOnEntityMerged : IConsumer<EntityMergedEvent>
    {
        private readonly IEntityLogService _entityLogService;

        public LoggingOnEntityMerged(IEntityLogService entityLogService)
        {
            _entityLogService = entityLogService;
        }

        public void HandleEvent(EntityMergedEvent eventMessage)
        {
            if (eventMessage.EntityMetadata.LogEnabled)
            {
                _entityLogService.MergedLog(eventMessage.Merged, eventMessage.Target, eventMessage.EntityMetadata, eventMessage.AttributeMetadatas);
            }
        }
    }
}