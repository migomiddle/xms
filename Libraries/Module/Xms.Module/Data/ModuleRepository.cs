using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Data;
using Xms.Data.Abstractions;

namespace Xms.Module.Data
{
    /// <summary>
    /// 模块仓储
    /// </summary>
    public class ModuleRepository : IModuleRepository
    {
        private readonly DataRepositoryBase<Domain.Module> _repository;

        public ModuleRepository(IOptionsMonitor<DataBaseOptions> options)
        {
            _repository = new DataRepositoryBase<Domain.Module>(options.CurrentValue);
        }

        #region implements

        /// <summary>
        /// 创建记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Create(Domain.Module entity)
        {
            return _repository.CreateObject(entity);
        }

        /// <summary>
        /// 批量创建记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public bool CreateMany(List<Domain.Module> entities)
        {
            return _repository.CreateMany(entities);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(Domain.Module entity)
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
        public bool Update(UpdateContext<Domain.Module> context)
        {
            return _repository.Update(context);
        }

        /// <summary>
        /// 查询记录数
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public long Count(QueryDescriptor<Domain.Module> q)
        {
            return _repository.Count(q);
        }

        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public PagedList<Domain.Module> QueryPaged(QueryDescriptor<Domain.Module> q)
        {
            return _repository.QueryPaged(q);
        }

        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public List<Domain.Module> Query(QueryDescriptor<Domain.Module> q)
        {
            return _repository.Query(q);
        }

        /// <summary>
        /// 查询一条记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Domain.Module FindById(Guid id)
        {
            return _repository.FindById(id);
        }

        /// <summary>
        /// 查询一条记录
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Domain.Module Find(Expression<Func<Domain.Module, bool>> predicate)
        {
            return _repository.Find(predicate);
        }

        #endregion implements
    }
}