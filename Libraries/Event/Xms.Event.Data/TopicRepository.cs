using Xms.Core.Data;
using Xms.Data;
using Xms.Event.Domain;

namespace Xms.Event.Data
{
    /// <summary>
    /// 事件主题仓储
    /// </summary>
    public class TopicRepository : DefaultRepository<Topic>, ITopicRepository
    {
        public TopicRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}