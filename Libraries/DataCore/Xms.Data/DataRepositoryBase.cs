using PetaPoco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
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
    public class DataRepositoryBase<T>
        where T : class
    {
        protected readonly IDataProvider<T> _repository;
        //protected readonly Type _entityType = typeof(T);

        //缓存执行语句
        //private static readonly ConcurrentDictionary<string, object> PocoExecuteContainer = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 实体元数据
        /// </summary>
        public PetaPoco.Core.PocoData MetaData
        {
            get
            {
                var md = PetaPoco.Core.PocoData.ForType(typeof(T), new DomainMapper());
                return md;
            }
        }

        //public IDataProviderOptions DataProviderOptions { get; set; }

        #region ctor

        public DataRepositoryBase(IDataProviderOptions options)
        {
            //DataProviderOptions = options;
            _repository = new DataProvider<T>(options);
        }

        public DataRepositoryBase(IDbContext context)
        {
            _repository = new DataProvider<T>(context);
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
            var result = _repository.Create(entity);
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
            var result = _repository.Create(entity);
            return true;
        }

        /// <summary>
        /// 批量创建记录
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public bool CreateMany(IEnumerable<T> entities)
        {
            var result = _repository.CreateMany(entities);
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
            ExecuteContext<T> ctx = new ExecuteContext<T>(s);
            var result = _repository.DeleteByQuery(ctx);
            return result;
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
            var result = _repository.Update(entity);
            return result;
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public bool Update(UpdateContext<T> context)
        {
            Guard.NotNullOrEmpty<KeyValuePair<string, object>>(context.Sets, "sets");

            ExecuteContext<T> ctx = new ExecuteContext<T>(PocoHelper.ParseUpdateSql<T>(MetaData, context));
            var result = _repository.UpdateByQuery(ctx);
            return result;
        }

        public void Save(T entity)
        {
            _repository.Save(entity);
        }

        #endregion 更新记录

        #region 查询记录

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="q">上下文</param>
        /// <returns></returns>
        public PagedList<T> QueryPaged(QueryDescriptor<T> q)
        {
            ExecuteContext<T> ctx = PocoHelper.ParseContext<T>(MetaData, q);
            var pageDatas = _repository.QueryPaged(ctx);
            if (pageDatas != null)
            {
                PagedList<T> list = new PagedList<T>()
                {
                    CurrentPage = pageDatas.CurrentPage
                    ,
                    ItemsPerPage = pageDatas.ItemsPerPage
                    ,
                    TotalItems = pageDatas.TotalItems
                    ,
                    TotalPages = pageDatas.TotalPages
                    ,
                    Items = pageDatas.Items
                };
                return list;
            }
            return null;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        public List<T> FindAll()
        {
            ExecuteContext<T> ctx = PocoHelper.ParseContext<T>(MetaData, null);
            var pageDatas = _repository.Query(ctx);
            if (pageDatas != null)
            {
                return pageDatas.ToList();
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
            ExecuteContext<T> ctx = PocoHelper.ParseContext<T>(MetaData, q);
            var pageDatas = _repository.Query(ctx);
            if (pageDatas != null)
            {
                return pageDatas.ToList();
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
            QueryDescriptor<T> q = QueryDescriptorBuilder.Build<T>();
            q.Where(predicate)
                .Sort(sorts);
            ExecuteContext<T> ctx = PocoHelper.ParseContext<T>(MetaData, q);
            var pageDatas = _repository.Query(ctx);
            if (pageDatas != null)
            {
                return pageDatas.ToList();
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
            ExecuteContext<T> ctx = PocoHelper.ParseContext<T>(q);
            var datas = _repository.Top(ctx);
            if (datas != null)
            {
                return datas.ToList();
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
            QueryDescriptor<T> q = QueryDescriptorBuilder.Build<T>();
            q.Where(predicate)
                .Sort(sorts);
            ExecuteContext<T> ctx = PocoHelper.ParseContext<T>(MetaData, q);
            ctx.TopCount = top;
            var pageDatas = _repository.Top(ctx);
            if (pageDatas != null)
            {
                return pageDatas.ToList();
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
            ExecuteContext<T> ctx;
            //缓存执行语句
            //if (PocoExecuteContainer.TryGetValue(key, out object value))
            //{
            //    ctx = new ExecuteContext<T>();
            //    Sql s = value as Sql;
            //    s.Arguments[0] = id;
            //    ctx.ExecuteContainer = s;
            //}
            //else
            //{
            QueryDescriptor<T> q = QueryDescriptorBuilder.Build<T>();
            q.QueryText = "[" + this.MetaData.TableInfo.TableName + "]." + this.MetaData.TableInfo.PrimaryKey + "=@0";
            q.Parameters.Add(new QueryParameter("@0", id));
            ctx = PocoHelper.ParseContext<T>(MetaData, q);
            //    PocoExecuteContainer[key] = ctx.ExecuteContainer;
            //}
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
            ExecuteContext<T> ctx = PocoHelper.ParseContext<T>(MetaData, q);
            Sql s = ctx.ExecuteContainer as Sql;
            string sql = s.SQL;
            sql = new Regex("^(SELECT){1}").Replace(sql, "SELECT TOP 1");
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
            ExecuteContext<T> ctx = new ExecuteContext<T>();
            sql = new Regex("^(SELECT){1}").Replace(sql, "SELECT TOP 1");
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
            ExecuteContext<T> ctx = PocoHelper.ParseContext<T>(MetaData, q, null, true);
            var result = _repository.Count(ctx);
            return result;
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
            Sql s = Sql.Builder.Append("SELECT TOP 1 1 AS result FROM [" + MetaData.TableInfo.TableName + "] ")
                .Append(PocoHelper.GetConditions(context));
            ExecuteContext<T> ctx = new ExecuteContext<T>(s);
            var result = _repository.Exists(ctx);
            return result;
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
            ExecuteContext<T> ctx = new ExecuteContext<T>(s);
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
            ExecuteContext<T> ctx = new ExecuteContext<T>
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
            ExecuteContext<T> ctx = new ExecuteContext<T>
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
            ExecuteContext<T> ctx = new ExecuteContext<T>
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
            ExecuteContext<T> ctx = new ExecuteContext<T>
            {
                ExecuteContainer = s
            };
            var pageDatas = _repository.Query(ctx);
            if (pageDatas != null)
            {
                return pageDatas.ToList();
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
            var result = ((Database)_repository.DbContext).Query<T>(s, args);
            return result.AsEnumerable().ToList();
        }

        /// <summary>
        /// 直接执行查询语句
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public PagedList<T> ExecuteQueryPaged(int page, int pageSize, string s, params object[] args)
        {
            var result = ((Database)_repository.DbContext).Page<T>(page, pageSize, s, args);
            var p = new PagedList<T>
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalPages = result.TotalPages,
                TotalItems = result.TotalItems,
                Items = result.Items
            };
            return p;
        }

        #endregion 执行
    }
}