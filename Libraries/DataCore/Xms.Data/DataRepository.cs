using PetaPoco;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Data.Provider;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;

namespace Xms.Data
{
    /// <summary>
    /// 数据处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TImpl"></typeparam>
    public class DataRepository<T, TImpl> //: DataRepositoryBase<TImpl>
        where T : class
        where TImpl : class, T
    {
        protected readonly IDataProvider<TImpl> _repository;
        protected readonly Type _entityType = typeof(TImpl);

        //缓存执行语句
        private static readonly ConcurrentDictionary<string, object> PocoExecuteContainer = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 实体元数据
        /// </summary>
        public PetaPoco.Core.PocoData MetaData
        {
            get
            {
                var md = PetaPoco.Core.PocoData.ForType(_entityType, new DomainMapper());
                return md;
            }
        }

        public IDataProviderOptions DataProviderOptions { get; set; }

        #region ctor

        public DataRepository(IDataProviderOptions options)
        {
            DataProviderOptions = options;
            _repository = new DataProvider<TImpl>(options);
        }

        public DataRepository(IDbContext context)
        {
            _repository = new DataProvider<TImpl>(context);
        }

        #endregion ctor

        #region context

        public IDbConnection GetConnection()
        {
            return (_repository.DbContext as Database).Connection;
        }

        public IDbContext GetDbContext()
        {
            return _repository.DbContext;
        }

        public void SetDbContext(IDbContext context)
        {
            _repository.DbContext = context;
        }

        public virtual void BeginTransaction()
        {
            _repository.BeginTransaction();
        }

        public virtual void CompleteTransaction()
        {
            _repository.CompleteTransaction();
        }

        public virtual void RollBackTransaction()
        {
            _repository.AbortTransaction();
        }

        public virtual ITransaction GetTransaction()
        {
            return (_repository.DbContext as Database).GetTransaction();
        }

        #endregion context

        #region 创建记录

        /// <summary>
        /// 创建一行记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>记录主键值</returns>
        public int Create(T entity)
        {
            var result = _repository.Create((TImpl)entity);
            int id = int.Parse(result.ToString());
            return id;
        }

        /// <summary>
        /// 创建一行记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>是否成功</returns>
        public bool CreateObject(T entity)
        {
            _repository.Create((TImpl)entity);
            return true;
        }

        /// <summary>
        /// 批量创建记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public bool CreateMany(List<T> entities)
        {
            List<TImpl> impls = new List<TImpl>(entities.Select(n => n as TImpl));
            var result = _repository.CreateMany(impls);
            return result;
        }

        #endregion 创建记录

        #region 删除记录

        /// <summary>
        /// 删除一行记录
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public bool Delete(Guid id)
        {
            var result = _repository.Delete(id);
            return result;
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="predicate">过滤条件</param>
        /// <returns></returns>
        public bool Delete(Expression<Func<T, bool>> predicate)
        {
            QueryDescriptor<T> q = QueryDescriptorBuilder.Build<T>();
            q.Where(predicate);
            Sql s = Sql.Builder.Append("DELETE [" + MetaData.TableInfo.TableName + "] ")
                .Append(PocoHelper.GetConditions(q.QueryText, q.Parameters));
            ExecuteContext<TImpl> ctx = new ExecuteContext<TImpl>(s);
            var result = _repository.DeleteByQuery(ctx);
            return result;
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="ids">主键</param>
        /// <returns></returns>
        public bool DeleteMany(List<Guid> ids)
        {
            if (ids.NotEmpty())
            {
                List<object> values = ids.Select(n => (object)n).ToList();
                var result = _repository.DeleteMany(values);
                return result;
            }
            return false;
        }

        #endregion 删除记录

        #region 更新记录

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public bool Update(T entity)
        {
            return _repository.Update((TImpl)entity);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public bool Update(UpdateContext<T> context)
        {
            Guard.NotNullOrEmpty(context.Sets, "sets");

            ExecuteContext<TImpl> ctx = new ExecuteContext<TImpl>(PocoHelper.ParseUpdateSql<T>(MetaData, context));
            var result = _repository.UpdateByQuery(ctx);
            return result;
        }

        public void Save(T entity)
        {
            _repository.Save((TImpl)entity);
        }

        #endregion 更新记录

        #region 查询记录

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        public List<T> FindAll()
        {
            ExecuteContext<TImpl> ctx = PocoHelper.ParseContext<T, TImpl>(MetaData, null);
            var pageDatas = _repository.Query(ctx);
            if (pageDatas != null)
            {
                return pageDatas.ToList<T>();
            }
            return null;
        }

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public PagedList<T> QueryPaged(QueryDescriptor<T> q)
        {
            ExecuteContext<TImpl> ctx = PocoHelper.ParseContext<T, TImpl>(MetaData, q);
            var result = _repository.QueryPaged(ctx);
            if (result != null)
            {
                PagedList<T> data = new PagedList<T>
                {
                    CurrentPage = result.CurrentPage,
                    Items = result.Items.ToList<T>(),
                    ItemsPerPage = result.ItemsPerPage,
                    TotalItems = result.TotalItems,
                    TotalPages = result.TotalPages
                };
                return data;
            }
            return null;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public List<T> Query(QueryDescriptor<T> q)
        {
            ExecuteContext<TImpl> ctx = PocoHelper.ParseContext<T, TImpl>(MetaData, q);
            var pageDatas = _repository.Query(ctx);
            if (pageDatas != null)
            {
                return pageDatas.ToList<T>();
            }
            return null;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="predicate">过滤条件</param>
        /// <param name="sorts">排序</param>
        /// <returns></returns>
        public List<T> Query(Expression<Func<T, bool>> predicate, params Func<SortDescriptor<T>, SortDescriptor<T>>[] sorts)
        {
            QueryDescriptor<T> qq = QueryDescriptorBuilder.Build<T>().Where(predicate).Sort(sorts);
            ////var prd = new DomainTypeModifier(typeof(TImpl)).Modify(predicate);
            //QueryDescriptor<TImpl> q = QueryBuilder.Build<TImpl>();
            ////query.Where((Expression<Func<TImpl, bool>>)prd);
            //q.QueryText = qq.QueryText;
            //q.Parameters = qq.Parameters;

            ExecuteContext<TImpl> ctx = PocoHelper.ParseContext<T, TImpl>(MetaData, qq);
            var pageDatas = _repository.Query(ctx);
            if (pageDatas != null)
            {
                return pageDatas.ToList<T>();
            }
            return null;
        }

        /// <summary>
        /// 查询前N条记录
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public List<T> Top(QueryDescriptor<T> q)
        {
            ExecuteContext<TImpl> ctx = PocoHelper.ParseContext<T, TImpl>(MetaData, q);
            var result = _repository.Top(ctx);
            if (result != null)
            {
                return result.ToList<T>();
            }
            return null;
        }

        /// <summary>
        /// 查询前N条记录
        /// </summary>
        /// <param name="predicate">过滤条件</param>
        /// <param name="sorts">排序</param>
        /// <returns></returns>
        public List<T> Top(int top, Expression<Func<T, bool>> predicate, params Func<SortDescriptor<T>, SortDescriptor<T>>[] sorts)
        {
            QueryDescriptor<T> q = QueryDescriptorBuilder.Build<T>()
                .Where(predicate)
                .Sort(sorts);
            ExecuteContext<TImpl> ctx = PocoHelper.ParseContext<T, TImpl>(MetaData, q);
            ctx.TopCount = top;
            var pageDatas = _repository.Top(ctx);
            if (pageDatas != null)
            {
                return pageDatas.ToList<T>();
            }
            return null;
        }

        /// <summary>
        /// 查询一行记录
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public T FindById(object id)
        {
            //return _repository.FindById(id);
            var key = MetaData.TableInfo.TableName + ".FindById";
            ExecuteContext<TImpl> ctx;
            //缓存执行语句
            if (PocoExecuteContainer.TryGetValue(key, out object value))
            {
                ctx = new ExecuteContext<TImpl>();
                Sql s = value as Sql;
                s.Arguments[0] = id;
                ctx.ExecuteContainer = s;
            }
            else
            {
                QueryDescriptor<T> q = QueryDescriptorBuilder.Build<T>();
                q.QueryText = PocoHelper.FormatColumn(this.MetaData, this.MetaData.TableInfo.PrimaryKey) + "=@0";
                q.Parameters.Add(new QueryParameter("@0", id));
                ctx = PocoHelper.ParseContext<T, TImpl>(MetaData, q);
                PocoExecuteContainer[key] = ctx.ExecuteContainer;
            }
            var result = _repository.Single(ctx);
            return result;
        }

        /// <summary>
        /// 查询一行记录
        /// </summary>
        /// <param name="predicate">过滤条件</param>
        /// <returns></returns>
        public T Find(Expression<Func<T, bool>> predicate)
        {
            QueryDescriptor<T> q = QueryDescriptorBuilder.Build<T>();
            q.Where(predicate);
            return Find(q);
        }

        /// <summary>
        /// 查询一行记录
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public T Find(QueryDescriptor<T> q)
        {
            ExecuteContext<TImpl> ctx = PocoHelper.ParseContext<T, TImpl>(MetaData, q);
            Sql s = ctx.ExecuteContainer as Sql;
            string sql = s.SQL;
            //sql = new Regex("^(SELECT){1}").Replace(sql, "SELECT TOP 1");
            ctx.ExecuteContainer = Sql.Builder.Append(sql, s.Arguments);
            var result = _repository.Single(ctx);
            return result;
        }

        /// <summary>
        /// 查询一行记录
        /// </summary>
        /// <param name="sql">上下文</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public T Find(string sql, params object[] args)
        {
            ExecuteContext<TImpl> ctx = new ExecuteContext<TImpl>();
            //sql = new Regex("^(SELECT){1}").Replace(sql, "SELECT TOP 1");
            ctx.ExecuteContainer = Sql.Builder.Append(sql, args);
            var result = _repository.Single(ctx);
            return result;
        }

        /// <summary>
        /// 查询记录数
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public long Count(QueryDescriptor<T> q)
        {
            ExecuteContext<TImpl> ctx = PocoHelper.ParseContext<T, TImpl>(MetaData, q);
            return _repository.Count(ctx);
        }

        #endregion 查询记录

        #region 是否存在

        /// <summary>
        /// 是否存在记录
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public bool Exists(QueryDescriptor<T> context)
        {
            Sql s = Sql.Builder.Append("SELECT COUNT(1) AS result FROM [" + MetaData.TableInfo.TableName + "] ")
                .Append(PocoHelper.GetConditions(context));
            ExecuteContext<TImpl> ctx = new ExecuteContext<TImpl>(s);
            var result = _repository.Count(ctx);
            return result > 0;
        }

        /// <summary>
        /// 是否存在记录
        /// </summary>
        /// <param name="predicate">过滤条件</param>
        /// <returns></returns>
        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            QueryDescriptor<T> q = QueryDescriptorBuilder.Build<T>();
            q.Where(predicate);
            return Exists(q);
        }

        /// <summary>
        /// 是否存在记录
        /// </summary>
        /// <param name="s">上下文</param>
        /// <returns></returns>
        public bool Exists(Sql s)
        {
            ExecuteContext<TImpl> ctx = new ExecuteContext<TImpl>(s);
            var result = _repository.Exists(ctx);
            return result;
        }

        #endregion 是否存在

        #region 执行

        /// <summary>
        /// 直接执行
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public int Execute(Sql s)
        {
            ExecuteContext<TImpl> ctx = new ExecuteContext<TImpl>
            {
                ExecuteContainer = s
            };
            var result = _repository.Execute(ctx);
            return result;
        }

        /// <summary>
        /// 直接执行
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public int Execute(string s, params object[] args)
        {
            Sql sql = new Sql(s, args);
            ExecuteContext<TImpl> ctx = new ExecuteContext<TImpl>
            {
                ExecuteContainer = sql
            };
            var result = _repository.Execute(ctx);
            return result;
        }

        /// <summary>
        /// 直接执行查询语句
        /// </summary>
        /// <param name="predicate">过滤条件</param>
        /// <param name="sorts">排序</param>
        /// <returns></returns>
        public T ExecuteFind(Sql s)
        {
            ExecuteContext<TImpl> ctx = new ExecuteContext<TImpl>
            {
                ExecuteContainer = s
            };
            var t = _repository.Single(ctx);
            return t;
        }

        /// <summary>
        /// 直接执行查询语句
        /// </summary>
        /// <param name="predicate">过滤条件</param>
        /// <param name="sorts">排序</param>
        /// <returns></returns>
        public List<T> ExecuteQuery(Sql s)
        {
            ExecuteContext<TImpl> ctx = new ExecuteContext<TImpl>
            {
                ExecuteContainer = s
            };
            var pageDatas = _repository.Query(ctx);
            if (pageDatas != null)
            {
                return pageDatas.ToList<T>();
            }
            return null;
        }

        /// <summary>
        /// 直接执行查询语句
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public List<T> ExecuteQuery(string s, params object[] args)
        {
            Sql sql = new Sql(s, args);
            ExecuteContext<TImpl> ctx = new ExecuteContext<TImpl>
            {
                ExecuteContainer = sql
            };
            var result = _repository.Query(ctx);
            return result.ToList<T>();
        }

        /// <summary>
        /// 直接执行查询语句
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public PagedList<T> ExecuteQueryPaged(int page, int pageSize, string s, params object[] args)
        {
            Sql sql = new Sql(s, args);
            ExecuteContext<TImpl> ctx = new ExecuteContext<TImpl>
            {
                ExecuteContainer = sql
                ,
                PagingInfo = new PageDescriptor { PageNumber = page, PageSize = pageSize }
            };
            var result = _repository.QueryPaged(ctx);
            var p = new PagedList<T>
            {
                CurrentPage = result.CurrentPage,
                Items = result.Items.ToList<T>(),
                ItemsPerPage = result.ItemsPerPage,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            };
            return p;
        }

        #endregion 执行
    }
}