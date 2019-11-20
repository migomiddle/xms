using Xms.Core.Data;

namespace Xms.Logging.AppLog.Data
{
    public interface IAppLogRepository : IRepository<Domain.VisitedLog>
    {
        void Clear();
    }
}