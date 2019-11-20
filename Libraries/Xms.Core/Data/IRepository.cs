using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;

namespace Xms.Core.Data
{
    public interface IRepository<T> where T : class
    {
        bool Create(T entity);

        bool CreateMany(IEnumerable<T> entities);

        bool Update(T entity);

        bool Update(UpdateContext<T> context);

        bool DeleteById(Guid id);

        bool DeleteMany(IEnumerable<Guid> ids);

        bool DeleteMany(Expression<Func<T, bool>> predicate);

        void Save(T entity);

        PagedList<T> QueryPaged(QueryDescriptor<T> q);

        List<T> Query(QueryDescriptor<T> q);

        List<T> Query(Expression<Func<T, bool>> predicate);

        List<T> Query(Expression<Func<T, bool>> predicate, params Func<SortDescriptor<T>, SortDescriptor<T>>[] sorts);

        List<T> FindAll();

        List<T> Top(QueryDescriptor<T> q);

        List<T> Top(int top, Expression<Func<T, bool>> predicate, params Func<SortDescriptor<T>, SortDescriptor<T>>[] sorts);

        T FindById(Guid id);

        T Find(Expression<Func<T, bool>> predicate);

        T Find(QueryDescriptor<T> q);

        long Count(QueryDescriptor<T> q);

        bool Exists(QueryDescriptor<T> context);

        bool Exists(Expression<Func<T, bool>> predicate);

        IDbContext DbContext { get; set; }
    }
}