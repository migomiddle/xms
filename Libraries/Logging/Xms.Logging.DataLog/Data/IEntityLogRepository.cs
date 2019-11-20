using System;
using Xms.Core.Data;
using Xms.Logging.DataLog.Domain;

namespace Xms.Logging.DataLog.Data
{
    public interface IEntityLogRepository : IRepository<EntityLog>
    {
        void Clear();

        void Clear(Guid entityId);
    }
}