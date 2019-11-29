using Xms.Core.Data;
using Xms.Data;
using Xms.Event.Domain;

namespace Xms.Event.Data
{
    /// <summary>
    /// 事件消费者仓储
    /// </summary>
    public class ConsumerRepository : DefaultRepository<Consumer>, IConsumerRepository
    {
        public ConsumerRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}