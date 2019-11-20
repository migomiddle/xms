using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;

namespace Xms.Schema.OptionSet
{
    public interface IOptionSetFinder
    {
        Domain.OptionSet Find(Expression<Func<Domain.OptionSet, bool>> predicate);

        Domain.OptionSet FindById(Guid id);

        List<Domain.OptionSet> FindAll();

        List<Domain.OptionSet> Query(Func<QueryDescriptor<Domain.OptionSet>, QueryDescriptor<Domain.OptionSet>> container);

        PagedList<Domain.OptionSet> QueryPaged(Func<QueryDescriptor<Domain.OptionSet>, QueryDescriptor<Domain.OptionSet>> container);

        PagedList<Domain.OptionSet> QueryPaged(Func<QueryDescriptor<Domain.OptionSet>, QueryDescriptor<Domain.OptionSet>> container, Guid solutionId, bool existInSolution);
    }
}