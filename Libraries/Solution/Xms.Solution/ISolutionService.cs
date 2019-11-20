using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;

namespace Xms.Solution
{
    public interface ISolutionService
    {
        bool Create(Domain.Solution entity);

        bool DeleteById(Guid id);

        bool DeleteById(IEnumerable<Guid> ids);

        Domain.Solution Find(Expression<Func<Domain.Solution, bool>> predicate);

        Domain.Solution FindById(Guid id);

        List<Domain.Solution> Query(Func<QueryDescriptor<Domain.Solution>, QueryDescriptor<Domain.Solution>> container);

        PagedList<Domain.Solution> QueryPaged(Func<QueryDescriptor<Domain.Solution>, QueryDescriptor<Domain.Solution>> container);

        bool Update(Func<UpdateContext<Domain.Solution>, UpdateContext<Domain.Solution>> context);

        bool Update(Domain.Solution entity);
    }
}