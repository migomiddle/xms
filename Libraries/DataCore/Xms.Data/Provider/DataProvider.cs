using PetaPoco;
using System.Collections.Generic;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;

namespace Xms.Data.Provider
{
    /// <summary>
    /// mssql 数据库操作类
    /// </summary>
    /// <typeparam name="T">entity</typeparam>
    public class DataProvider<T> : IDataProvider<T> where T : class
    {
        private readonly IDataProviderOptions _providerOptions;
        private bool ConnectionChanged { get; set; }

        public string DataBaseName { get; set; }
        private string _connectionString;

        public string ConnectionString
        {
            get { return _connectionString; }
            private set
            {
                ConnectionChanged = !_connectionString.IsCaseInsensitiveEqual(value);
                _connectionString = value;
            }
        }

        /// <summary>
        /// 查询上下文
        /// </summary>
        public IExecuteContext<T> CurrentContext { get; private set; }

        /// <summary>
        /// 最大查询数
        /// </summary>
        public int MaxSearchCount = 25000;

        private IDbContext _dbContext;

        public IDbContext DbContext
        {
            get
            {
                if (_dbContext == null || ConnectionChanged)
                {
                    if (ConnectionString.IsNotEmpty())
                    {
                        if (_providerOptions != null)
                        {
                            _dbContext = new DbContext(_providerOptions);
                        }
                        else
                        {
                            var config = new XmsDbConfiguration(null);
                            config.ConnectionString = ConnectionString;
                            _dbContext = new DbContext(_providerOptions ?? config);
                        }
                        ConnectionChanged = false;
                    }
                    else
                    {
                        throw new XmsException("ConnectionString is empty");
                    }
                }
                return _dbContext;
            }
            set
            {
                _dbContext = value;
            }
        }

        public DbContext DataBase
        {
            get
            {
                var db = DbContext as DbContext;
                //if(db.Connection.State == System.Data.ConnectionState.Closed)
                //{
                //    ConnectionChanged = true;
                //    return DbContext as DbContext;
                //}
                return db;
            }
            set
            {
                DbContext = value;
            }
        }

        public DataProvider(IDataProviderOptions options)
        {
            _providerOptions = options;
            ConnectionString = _providerOptions.ConnectionString;
        }

        public DataProvider(IDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual void BeginTransaction()
        {
            DbContext.BeginTransaction();
        }

        public virtual void CompleteTransaction()
        {
            DbContext.CompleteTransaction();
        }

        public virtual void AbortTransaction()
        {
            DbContext.RollBackTransaction();
        }

        public bool Exists(IExecuteContext<T> context)
        {
            this.CurrentContext = context;
            var result = DataBase.FirstOrDefault<T>(GetExecuteContainer());
            return result != null;
        }

        public long Count(IExecuteContext<T> context)
        {
            this.CurrentContext = context;
            long count = DataBase.ExecuteScalar<long>(GetExecuteContainer());
            return count;
        }

        public object Create(T entity)
        {
            object newid = DataBase.Insert(entity);
            return newid;
        }

        public bool CreateMany(IEnumerable<T> entities)
        {
            DbContext.BeginTransaction();
            foreach (var item in entities)
            {
                DataBase.Insert(item);
            }
            DbContext.CompleteTransaction();
            return true;
        }

        public bool Update(T entity)
        {
            int result = DataBase.Update(entity);
            return result > 0;
        }

        public bool UpdateMany(IEnumerable<T> entities)
        {
            DataBase.BeginTransaction();
            foreach (var item in entities)
            {
                DataBase.Update(item);
            }
            DataBase.CompleteTransaction();
            return true;
        }

        public bool UpdateByQuery(IExecuteContext<T> context)
        {
            this.CurrentContext = context;
            int result = DataBase.Execute(GetExecuteContainer());
            return result > 0;
        }

        public bool Delete(object id)
        {
            int result = DataBase.Delete<T>(id);
            return result > 0;
        }

        public bool DeleteMany(IEnumerable<object> ids)
        {
            DbContext.BeginTransaction();
            foreach (var id in ids)
            {
                DataBase.Delete<T>(id);
            }
            DbContext.CompleteTransaction();
            return true;
        }

        public bool DeleteByQuery(IExecuteContext<T> context)
        {
            this.CurrentContext = context;
            int result = DataBase.Execute(GetExecuteContainer());
            return result > 0;
        }

        public T FindById(object id)
        {
            return DataBase.SingleOrDefault<T>(id);
        }

        public T Single(IExecuteContext<T> context)
        {
            this.CurrentContext = context;
            return DataBase.SingleOrDefault<T>(GetExecuteContainer());
        }

        public PagedList<T> QueryPaged(IExecuteContext<T> context)
        {
            this.CurrentContext = context;
            Page<T> result = null;
            if (null != context.PagingInfo)
            {
                PageDescriptor p = context.PagingInfo;
                result = DataBase.Page<T>(p.PageNumber, p.PageSize, GetExecuteContainer());
            }
            else
            {
                result = DataBase.Page<T>(1, context.TopCount > 0 ? context.TopCount : MaxSearchCount, GetExecuteContainer());
            }
            return new PagedList<T>
            {
                CurrentPage = result.CurrentPage
                ,
                Items = result.Items
                ,
                ItemsPerPage = result.ItemsPerPage
                ,
                TotalItems = result.TotalItems
                ,
                TotalPages = result.TotalPages
            };
        }

        public List<T> Query(IExecuteContext<T> context)
        {
            this.CurrentContext = context;
            return DataBase.Fetch<T>(GetExecuteContainer());
        }

        public void Save(T entity)
        {
            DataBase.Save(entity);
        }

        public List<T> Top(IExecuteContext<T> context)
        {
            this.CurrentContext = context;
            return DataBase.SkipTake<T>(0, context.TopCount > 0 ? context.TopCount : MaxSearchCount, GetExecuteContainer());
        }

        public int Execute(IExecuteContext<T> context)
        {
            this.CurrentContext = context;
            return DataBase.Execute(GetExecuteContainer());
        }

        public Sql GetExecuteContainer()
        {
            Sql s = this.CurrentContext.ExecuteContainer as Sql;

            //var args = new List<string>();
            //foreach (var arg in s.Arguments)
            //{
            //    if (arg is AnsiString)
            //    {
            //        args.Add((arg as AnsiString).Value);
            //    }
            //}
            //System.Diagnostics.Debug.WriteLine(s.SQL + ";" + string.Join(",", args), "sql");

            return s;
        }
    }
}