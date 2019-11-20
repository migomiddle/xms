using System;
using System.Collections.Generic;
using Xms.Core.Data;
using Xms.Sdk.Query;

namespace Xms.Sdk.Data
{
    public interface IOrganizationDataProvider
    {
        void BeginTransaction();

        void CommitTransaction();

        void RollBackTransaction();

        /// <summary>
        /// 新建记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Guid Create(Entity entity);

        /// <summary>
        /// 批量新建记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        bool CreateMany(IList<Entity> entities);

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Update(Entity entity);

        /// <summary>
        /// 通过查询更新一批记录
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        bool Update(Entity entity, IQueryResolver queryResolver, bool ignorePermissions = false);

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(string name, Guid id, string primarykey = "");
    }
}