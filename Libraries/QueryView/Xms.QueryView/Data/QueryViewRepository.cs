using System;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure.Utility;

namespace Xms.QueryView.Data
{
    /// <summary>
    /// 视图仓储
    /// </summary>
    public class QueryViewRepository : DefaultRepository<Domain.QueryView>, IQueryViewRepository
    {
        public QueryViewRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region implements

        public PagedList<Domain.QueryView> QueryPaged(QueryDescriptor<Domain.QueryView> q, int solutionComponentType, Guid solutionId, bool existInSolution)
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