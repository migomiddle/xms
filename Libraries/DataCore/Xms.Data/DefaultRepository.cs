using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;

namespace Xms.Data
{
    /// <summary>
    /// 具有一组默认行为的仓储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TImpl"></typeparam>
    public abstract class DefaultRepository<T, TImpl> : IRepository<T>
        where T : class
        where TImpl : class, T
    {
        protected readonly DataRepository<T, TImpl> _repository;

        public DefaultRepository(IDbContext dbContext)
        {
            _repository = new DataRepository<T, TImpl>(dbContext);
        }

        public virtual IDbContext DbContext
        {
            get
            {
                return _repository.GetDbContext();
            }
            set
            {
                _repository.SetDbContext(value);
            }
        }

        public virtual PetaPoco.Database Database
        {
            get { return DbContext as PetaPoco.Database; }
        }

        /// <summary>
        /// 实体元数据
        /// </summary>
        public virtual PetaPoco.Core.PocoData MetaData
        {
            get
            {
                return _repository.MetaData;
            }
        }

        /// <summary>
        /// 实体表名
        /// </summary>
        public virtual string TableName
        {
            get
            {
                return MetaData.TableInfo.TableName;
            }
        }

        /// <summary>
        /// 查询记录数
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public virtual long Count(QueryDescriptor<T> q)
        {
            return _repository.Count(q);
        }

        /// <summary>
        /// 创建记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Create(T entity)
        {
            return _repository.CreateObject(entity);
        }

        public virtual bool CreateMany(IEnumerable<T> entities)
        {
            return _repository.CreateMany(entities.ToList());
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool DeleteById(Guid id)
        {
            return _repository.Delete(id);
        }

        public virtual bool DeleteMany(IEnumerable<Guid> ids)
        {
            return _repository.DeleteMany(ids.ToList());
        }

        public virtual bool DeleteMany(Expression<Func<T, bool>> predicate)
        {
            return _repository.Delete(predicate);
        }

        public virtual bool Exists(QueryDescriptor<T> context)
        {
            return _repository.Exists(context);
        }

        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            return _repository.Exists(predicate);
        }

        /// <summary>
        /// 查询一条记录
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual T Find(Expression<Func<T, bool>> predicate)
        {
            return _repository.Find(predicate);
        }

        public virtual T Find(QueryDescriptor<T> q)
        {
            return _repository.Find(q);
        }

        public virtual List<T> FindAll()
        {
            return _repository.FindAll();
        }

        /// <summary>
        /// 查询一条记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T FindById(Guid id)
        {
            return _repository.FindById(id);
        }

        public virtual T FindById(object id)
        {
            return _repository.FindById(id);
        }

        public virtual List<T> Query(QueryDescriptor<T> q)
        {
            return _repository.Query(q);
        }

        public virtual List<T> Query(Expression<Func<T, bool>> predicate)
        {
            QueryDescriptor<T> q = QueryDescriptorBuilder.Build<T>();
            q.Where(predicate);
            return _repository.Query(q);
        }

        public virtual List<T> Query(Expression<Func<T, bool>> predicate, params Func<SortDescriptor<T>, SortDescriptor<T>>[] sorts)
        {
            return _repository.Query(predicate, sorts);
        }

        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public virtual PagedList<T> QueryPaged(QueryDescriptor<T> q)
        {
            return _repository.QueryPaged(q);
        }

        /// <summary>
        /// 保存记录
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Save(T entity)
        {
            _repository.Save(entity);
        }

        /// <summary>
        /// 获取前X条记录
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public virtual List<T> Top(QueryDescriptor<T> q)
        {
            return _repository.Top(q);
        }

        public virtual List<T> Top(int top, Expression<Func<T, bool>> predicate, params Func<SortDescriptor<T>, SortDescriptor<T>>[] sorts)
        {
            return _repository.Top(top, predicate, sorts);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Update(T entity)
        {
            return _repository.Update(entity);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual bool Update(UpdateContext<T> context)
        {
            return _repository.Update(context);
        }
    }

    /// <summary>
    /// 具有一组默认行为的仓储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TImpl"></typeparam>
    public abstract class DefaultRepository<T> : IRepository<T>
        where T : class
    {
        protected readonly DataRepositoryBase<T> _repository;

        public DefaultRepository(IDbContext dbContext)
        {
            _repository = new DataRepositoryBase<T>(dbContext);
        }

        public virtual IDbContext DbContext
        {
            get
            {
                return _repository.GetDbContext();
            }
            set
            {
                _repository.SetDbContext(value);
            }
        }

        public virtual PetaPoco.Database Database
        {
            get { return DbContext as PetaPoco.Database; }
        }

        /// <summary>
        /// 实体元数据
        /// </summary>
        public virtual PetaPoco.Core.PocoData MetaData
        {
            get
            {
                return _repository.MetaData;
            }
        }

        /// <summary>
        /// 实体表名
        /// </summary>
        public virtual string TableName
        {
            get
            {
                return MetaData.TableInfo.TableName;
            }
        }

        /// <summary>
        /// 查询记录数
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public virtual long Count(QueryDescriptor<T> q)
        {
            return _repository.Count(q);
        }

        /// <summary>
        /// 创建记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Create(T entity)
        {
            return _repository.CreateObject(entity);
        }

        public virtual bool CreateMany(IEnumerable<T> entities)
        {
            return _repository.CreateMany(entities.ToList());
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool DeleteById(Guid id)
        {
            return _repository.Delete(id);
        }

        public virtual bool DeleteMany(IEnumerable<Guid> ids)
        {
            return _repository.DeleteMany(ids.ToList());
        }

        public virtual bool DeleteMany(Expression<Func<T, bool>> predicate)
        {
            return _repository.Delete(predicate);
        }

        public virtual bool Exists(QueryDescriptor<T> context)
        {
            return _repository.Exists(context);
        }

        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            return _repository.Exists(predicate);
        }

        /// <summary>
        /// 查询一条记录
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual T Find(Expression<Func<T, bool>> predicate)
        {
            return _repository.Find(predicate);
        }

        public virtual T Find(QueryDescriptor<T> q)
        {
            return _repository.Find(q);
        }

        public virtual List<T> FindAll()
        {
            return _repository.FindAll();
        }

        /// <summary>
        /// 查询一条记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T FindById(Guid id)
        {
            return _repository.FindById(id);
        }

        public virtual T FindById(object id)
        {
            return _repository.FindById(id);
        }

        public virtual List<T> Query(QueryDescriptor<T> q)
        {
            return _repository.Query(q);
        }

        public virtual List<T> Query(Expression<Func<T, bool>> predicate)
        {
            QueryDescriptor<T> q = QueryDescriptorBuilder.Build<T>();
            q.Where(predicate);
            return _repository.Query(q);
        }

        public virtual List<T> Query(Expression<Func<T, bool>> predicate, params Func<SortDescriptor<T>, SortDescriptor<T>>[] sorts)
        {
            return _repository.Query(predicate, sorts);
        }

        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public virtual PagedList<T> QueryPaged(QueryDescriptor<T> q)
        {
            return _repository.QueryPaged(q);
        }

        /// <summary>
        /// 保存记录
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Save(T entity)
        {
            _repository.Save(entity);
        }

        /// <summary>
        /// 获取前X条记录
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public virtual List<T> Top(QueryDescriptor<T> q)
        {
            return _repository.Top(q);
        }

        public virtual List<T> Top(int top, Expression<Func<T, bool>> predicate, params Func<SortDescriptor<T>, SortDescriptor<T>>[] sorts)
        {
            return _repository.Top(top, predicate, sorts);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Update(T entity)
        {
            return _repository.Update(entity);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual bool Update(UpdateContext<T> context)
        {
            return _repository.Update(context);
        }
    }
}