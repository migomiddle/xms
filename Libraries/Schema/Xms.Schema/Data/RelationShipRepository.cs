using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;

namespace Xms.Schema.Data
{
    /// <summary>
    /// 关系元数据仓储
    /// </summary>
    public class RelationShipRepository : DefaultRepository<Domain.RelationShip>, IRelationShipRepository
    {
        public RelationShipRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region implements

        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public override PagedList<Domain.RelationShip> QueryPaged(QueryDescriptor<Domain.RelationShip> q)
        {
            //this.TableName = "RelationShipView";
            Sql s = Sql.Builder.Append("SELECT " + (q.Columns.NotEmpty() ? string.Join(",", q.Columns) : "*") + " FROM RelationShipView AS RelationShip");
            if (q.QueryText.IsNotEmpty())
            {
                s.Append("WHERE").Append(q.QueryText, q.Parameters.Select(n => n.Value).ToArray());
            }
            return _repository.ExecuteQueryPaged(q.PagingDescriptor.PageNumber, q.PagingDescriptor.PageSize, s.SQL, s.Arguments);
            //return _repository.QueryPaged(q);
        }

        public override List<Domain.RelationShip> Query(QueryDescriptor<Domain.RelationShip> q)
        {
            //this.TableName = "RelationShipView";
            Sql s = Sql.Builder.Append("SELECT " + (q.Columns.NotEmpty() ? string.Join(",", q.Columns) : "*") + " FROM RelationShipView AS RelationShip");
            if (q.QueryText.IsNotEmpty())
            {
                s.Append("WHERE").Append(q.QueryText, q.Parameters.Select(n => n.Value).ToArray());
            }
            return _repository.ExecuteQuery(s);
        }

        public override List<Domain.RelationShip> Query(Expression<Func<Domain.RelationShip, bool>> predicate)
        {
            QueryDescriptor<Domain.RelationShip> q = QueryDescriptorBuilder.Build<Domain.RelationShip>();
            q.Where(predicate);
            Sql s = Sql.Builder.Append("SELECT " + (q.Columns.NotEmpty() ? string.Join(",", q.Columns) : "*") + " FROM RelationShipView AS RelationShip");
            if (q.QueryText.IsNotEmpty())
            {
                s.Append("WHERE").Append(q.QueryText, q.Parameters.Select(n => n.Value).ToArray());
            }
            return _repository.ExecuteQuery(s);
        }

        public override List<Domain.RelationShip> FindAll()
        {
            Sql s = Sql.Builder.Append("SELECT * FROM RelationShipView");
            return _repository.ExecuteQuery(s);
        }

        /// <summary>
        /// 查询一条记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Domain.RelationShip FindById(Guid id)
        {
            return _repository.Find("SELECT * FROM RelationShipView WHERE RelationShipId=@0", id);
        }

        /// <summary>
        /// 查询一条记录
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public override Domain.RelationShip Find(Expression<Func<Domain.RelationShip, bool>> predicate)
        {
            QueryDescriptor<Domain.RelationShip> q = QueryDescriptorBuilder.Build<Domain.RelationShip>();
            q.Where(predicate);
            string s = "SELECT * FROM RelationShipView AS RelationShip";
            if (q.QueryText.IsNotEmpty())
            {
                s += " WHERE " + q.QueryText;
            }
            return _repository.Find(s, q.Parameters.Select(n => n.Value).ToArray());//_repository.Find(predicate);
        }

        #endregion implements
    }
}