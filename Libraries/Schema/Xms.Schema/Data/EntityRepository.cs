using PetaPoco;
using System;
using System.Collections.Generic;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data;
using Xms.Data.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;

namespace Xms.Schema.Data
{
    /// <summary>
    /// 实体元数据仓储
    /// </summary>
    public class EntityRepository : DefaultRepository<Domain.Entity>, IEntityRepository
    {
        private readonly IMetadataProvider _metadataProvider;

        public EntityRepository(IDbContext dbContext, IMetadataProvider metadataProvider) : base(dbContext)
        {
            _metadataProvider = metadataProvider;
        }

        #region implements

        /// <summary>
        /// 创建记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Create(Domain.Entity entity, List<Domain.Attribute> defaultAttributes, List<Domain.RelationShip> defaultRelationShips)
        {
            var flag = false;
            using (UnitOfWork.Build(DbContext))
            {
                flag = base.Create(entity);
                //创建数据库表
                _metadataProvider.CreateTable(entity, defaultAttributes);
                //创建SQL视图
                _metadataProvider.AlterView(entity, defaultAttributes, defaultRelationShips);
            }
            return flag;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool DeleteById(Guid id)
        {
            return this.DeleteById(id, true);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dropTable">是否删除数据库中的表</param>
        /// <returns></returns>
        public bool DeleteById(Guid id, bool dropTable)
        {
            var flag = true;
            using (UnitOfWork.Build(DbContext))
            {
                var entity = base.FindById(id);
                flag = base.DeleteById(id);
                if (dropTable)
                {
                    //删除物理表
                    _metadataProvider.DropTable(entity);
                    _metadataProvider.DropView(entity);
                }
            }
            return flag;
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteById(IEnumerable<Guid> ids)
        {
            var flag = false;
            foreach (var id in ids)
            {
                flag = this.DeleteById(id);
            }

            return flag;
        }

        /// <summary>
        /// 查询相关实体
        /// </summary>
        /// <param name="entityid"></param>
        /// <returns></returns>
        public IEnumerable<Domain.Entity> QueryRelated(Guid entityid, RelationShipType type, int cascadeLinkMask = -1)
        {
            Sql s = Sql.Builder.Append("SELECT * FROM Entity ");
            if (type == RelationShipType.ManyToMany || type == RelationShipType.ManyToOne)
            {
                if (cascadeLinkMask > 0)
                {
                    s.Append("WHERE EntityId IN (SELECT ReferencedEntityId FROM RelationShip WHERE ReferencingEntityId = @0 AND CascadeLinkMask = @1)", entityid, cascadeLinkMask);
                }
                else
                {
                    s.Append("WHERE EntityId IN (SELECT ReferencedEntityId FROM RelationShip WHERE ReferencingEntityId = @0)", entityid);
                }
            }
            else if (type == RelationShipType.OneToOne || type == RelationShipType.OneToMany)
            {
                if (cascadeLinkMask > 0)
                {
                    s.Append("WHERE EntityId IN (SELECT ReferencingEntityId FROM RelationShip WHERE ReferencedEntityId = @0 AND CascadeLinkMask = @1)", entityid, cascadeLinkMask);
                }
                else
                {
                    s.Append("WHERE EntityId IN (SELECT ReferencingEntityId FROM RelationShip WHERE ReferencedEntityId = @0)", entityid);
                }
            }

            return Database.Query<Domain.Entity>(s);
        }

        public PagedList<Domain.Entity> QueryPaged(QueryDescriptor<Domain.Entity> q, int solutionComponentType, Guid solutionId, bool existInSolution)
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