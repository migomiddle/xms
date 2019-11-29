using System;
using Xms.Core.Data;
using Xms.Data;
using Xms.Flow.Domain;

namespace Xms.Flow.Data
{
    /// <summary>
    /// 流程仓储
    /// </summary>
    public class WorkFlowProcessLogRepository : DefaultRepository<WorkFlowProcessLog>, IWorkFlowProcessLogRepository
    {
        public WorkFlowProcessLogRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region implements

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public bool DeleteByParentId(Guid parentid)
        {
            return _repository.Delete(n => n.WorkFlowInstanceId == parentid);
        }

        #endregion implements
    }
}