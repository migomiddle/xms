using System;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure.Utility;

namespace Xms.Schema.Data
{
    /// <summary>
    /// 选项集仓储
    /// </summary>
    public class OptionSetRepository : DefaultRepository<Domain.OptionSet>, IOptionSetRepository
    {
        public OptionSetRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public PagedList<Domain.OptionSet> QueryPaged(QueryDescriptor<Domain.OptionSet> q, int solutionComponentType, Guid solutionId, bool existInSolution)
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
    }
}