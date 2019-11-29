using System;
using Xms.Business.FormStateRule.Domain;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure.Utility;

namespace Xms.Business.FormStateRule.Data
{
    /// <summary>
    /// 表单状态控制规则仓储
    /// </summary>
    public class SystemFormStateRuleRepository : DefaultRepository<SystemFormStateRule>, ISystemFormStateRuleRepository
    {
        public SystemFormStateRuleRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region implements

        public PagedList<SystemFormStateRule> QueryPaged(QueryDescriptor<SystemFormStateRule> q, int solutionComponentType, Guid solutionId, bool existInSolution)
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