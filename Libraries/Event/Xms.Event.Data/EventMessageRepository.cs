using Xms.Core.Data;
using Xms.Data;
using Xms.Event.Domain;

namespace Xms.Event.Data
{
    /// <summary>
    /// 事件消息仓储
    /// </summary>
    public class EventMessageRepository : DefaultRepository<EventMessage>, IEventMessageRepository
    {
        public EventMessageRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}