using System;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure.Utility;
using Xms.Plugin.Domain;

namespace Xms.Plugin.Data
{
    /// <summary>
    /// 实体插件仓储
    /// </summary>
    public class EntityPluginRepository : DefaultRepository<EntityPlugin>, IEntityPluginRepository
    {
        public EntityPluginRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region implements

        public PagedList<EntityPlugin> QueryPaged(QueryDescriptor<EntityPlugin> q, int solutionComponentType, Guid solutionId, bool existInSolution)
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