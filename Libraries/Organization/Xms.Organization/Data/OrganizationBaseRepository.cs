using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Data;
using Xms.Data.Abstractions;
using Xms.Organization.Domain;

namespace Xms.Organization.Data
{
    /// <summary>
    /// 组织基础信息仓储
    /// </summary>
    public class OrganizationBaseRepository : IOrganizationBaseRepository
    {
        private readonly DataRepositoryBase<OrganizationBase> _repository;

        public OrganizationBaseRepository(IOptionsMonitor<DataBaseOptions> options)
        {
            _repository = new DataRepositoryBase<OrganizationBase>(options.CurrentValue);
        }

        public OrganizationBaseRepository(DataBaseOptions options)
        {
            _repository = new DataRepositoryBase<OrganizationBase>(options);
        }

        #region implements

        /// <summary>
        /// 创建记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Create(OrganizationBase entity)
        {
            return _repository.CreateObject(entity);
        }

        /// <summary>
        /// 批量创建记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public bool CreateMany(List<OrganizationBase> entities)
        {
            return _repository.CreateMany(entities);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(OrganizationBase entity)
        {
            return _repository.Update(entity);
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteById(Guid id)
        {
            return _repository.Delete(id);
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteById(List<Guid> ids)
        {
            return _repository.DeleteMany(ids);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public bool Update(UpdateContext<OrganizationBase> context)
        {
            return _repository.Update(context);
        }

        /// <summary>
        /// 查询记录数
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public long Count(QueryDescriptor<OrganizationBase> q)
        {
            return _repository.Count(q);
        }

        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public PagedList<OrganizationBase> QueryPaged(QueryDescriptor<OrganizationBase> q)
        {
            return _repository.QueryPaged(q);
        }

        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public List<OrganizationBase> Query(QueryDescriptor<OrganizationBase> q)
        {
            return _repository.Query(q);
        }

        /// <summary>
        /// 查询一条记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OrganizationBase FindById(Guid id)
        {
            return _repository.FindById(id);
        }

        /// <summary>
        /// 查询一条记录
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public OrganizationBase Find(Expression<Func<OrganizationBase, bool>> predicate)
        {
            return _repository.Find(predicate);
        }

        #endregion implements
    }
}