using System.Collections.Generic;
using Xms.Core.Context;

namespace Xms.Core.Data
{
    public interface IDataProvider<T>
     where T : class
    {
        IDbContext DbContext { get; set; }

        void BeginTransaction();

        void CompleteTransaction();

        void AbortTransaction();

        bool Exists(IExecuteContext<T> context);

        long Count(IExecuteContext<T> context);

        object Create(T entity);

        bool CreateMany(IEnumerable<T> entities);

        bool Update(T entity);

        bool UpdateMany(IEnumerable<T> entities);

        bool UpdateByQuery(IExecuteContext<T> context);

        bool Delete(object id);

        bool DeleteMany(IEnumerable<object> ids);

        bool DeleteByQuery(IExecuteContext<T> context);

        T FindById(object id);

        void Save(T entity);

        T Single(IExecuteContext<T> context);

        PagedList<T> QueryPaged(IExecuteContext<T> context);

        List<T> Top(IExecuteContext<T> context);

        List<T> Query(IExecuteContext<T> context);

        int Execute(IExecuteContext<T> context);
    }
}