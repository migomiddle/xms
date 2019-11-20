using System;
using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure.Utility;
using Xms.Logging.DataLog.Domain;

namespace Xms.Logging.DataLog.Data
{
    /// <summary>
    /// 实体数据日志仓储
    /// </summary>
    public class EntityLogRepository : DefaultRepository<EntityLog>, IEntityLogRepository
    {
        public EntityLogRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region Implements

        public void Clear()
        {
            _repository.Execute("TRUNCATE TABLE [{0}]".FormatWith(TableName));
        }

        public void Clear(Guid entityId)
        {
            _repository.Execute("DELETE [{0}] WHERE [EntityId]=@0".FormatWith(TableName), entityId);
        }

        #endregion Implements
    }
}