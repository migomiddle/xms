using System;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data;
using Xms.Flow.Domain;
using Xms.Infrastructure.Utility;

namespace Xms.Flow.Data
{
    /// <summary>
    /// 流程仓储
    /// </summary>
    public class WorkFlowRepository : DefaultRepository<WorkFlow>, IWorkFlowRepository
    {
        public WorkFlowRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region implements

        public PagedList<WorkFlow> QueryPaged(QueryDescriptor<WorkFlow> q, int solutionComponentType, Guid solutionId, bool existInSolution)
        {
            if (q.QueryText.IsNotEmpty())
            {
                q.QueryText += " AND ";
            }
            q.QueryText += MetaData.TableInfo.PrimaryKey + " " + (existInSolution ? "" : "NOT") + " IN(SELECT ObjectId FROM SolutionComponent WHERE SolutionId=@" + q.Parameters.Count;
            q.Parameters.Add(new QueryParameter("@" + q.Parameters.Count, solutionId));
            q.QueryText += " and ComponentType = @" + q.Parameters.Count;
            q.Parameters.Add(new QueryParameter("@" + q.Parameters.Count, solutionComponentType));
            q.QueryText += ")";
            return base.QueryPaged(q);
        }

        #endregion implements
    }
}