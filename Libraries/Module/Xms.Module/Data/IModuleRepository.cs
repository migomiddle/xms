using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;

namespace Xms.Module.Data
{
    public interface IModuleRepository
    {
        bool Create(Domain.Module entity);

        bool CreateMany(List<Domain.Module> entities);

        bool Update(Domain.Module entity);

        bool Update(UpdateContext<Domain.Module> context);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        long Count(QueryDescriptor<Domain.Module> q);

        PagedList<Domain.Module> QueryPaged(QueryDescriptor<Domain.Module> q);

        List<Domain.Module> Query(QueryDescriptor<Domain.Module> q);

        Domain.Module FindById(Guid id);

        Domain.Module Find(Expression<Func<Domain.Module, bool>> predicate);
    }
}