using System;
using Xms.Business.DuplicateValidator.Domain;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure.Utility;

namespace Xms.Business.DuplicateValidator.Data
{
    /// <summary>
    /// 重复检测规则仓储
    /// </summary>
    public class DuplicateRuleRepository : DefaultRepository<DuplicateRule>, IDuplicateRuleRepository
    {
        public DuplicateRuleRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region implements

        public PagedList<DuplicateRule> QueryPaged(QueryDescriptor<DuplicateRule> q, int solutionComponentType, Guid solutionId, bool existInSolution)
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