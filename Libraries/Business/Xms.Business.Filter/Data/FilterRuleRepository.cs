using System;
using Xms.Business.Filter.Domain;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure.Utility;

namespace Xms.Business.Filter.Data
{
    /// <summary>
    /// 拦截规则仓储
    /// </summary>
    public class FilterRuleRepository : DefaultRepository<FilterRule>, IFilterRuleRepository
    {
        public FilterRuleRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region implements

        public PagedList<FilterRule> QueryPaged(QueryDescriptor<FilterRule> q, int solutionComponentType, Guid solutionId, bool existInSolution)
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