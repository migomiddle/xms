using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure.Utility;

namespace Xms.Logging.AppLog.Data
{
    /// <summary>
    /// 访问日志仓储
    /// </summary>
    public class AppLogRepository : DefaultRepository<Domain.VisitedLog>, IAppLogRepository
    {
        public AppLogRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region Implements

        public void Clear()
        {
            _repository.Execute("TRUNCATE TABLE [{0}]".FormatWith(TableName));
        }

        #endregion Implements
    }
}