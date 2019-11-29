using System;
using System.Data;
using System.Data.SqlClient;
using Xms.Business.SerialNumber.Domain;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure.Utility;

namespace Xms.Business.SerialNumber.Data
{
    /// <summary>
    /// 自动编号规则仓储
    /// </summary>
    public class SerialNumberRuleRepository : DefaultRepository<SerialNumberRule>, ISerialNumberRuleRepository
    {
        public SerialNumberRuleRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region implements

        public string GetSerialNumber(Guid ruleid)
        {
            var param = new SqlParameter("num", SqlDbType.NVarChar, 100)
            {
                Direction = ParameterDirection.Output
            };

            _repository.Execute("EXECUTE usp_GetSerialNumber @ruleid,@num OUTPUT"
                , new { ruleid = ruleid, num = param });

            if (param.Value != null)
            {
                return param.Value.ToString();
            }

            return string.Empty;
        }

        public PagedList<SerialNumberRule> QueryPaged(QueryDescriptor<SerialNumberRule> q, int solutionComponentType, Guid solutionId, bool existInSolution)
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