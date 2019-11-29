using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Flow.Abstractions;
using Xms.Flow.Domain;

namespace Xms.Flow
{
    public interface IWorkFlowFinder
    {
        WorkFlow Find(Expression<Func<WorkFlow, bool>> predicate);

        WorkFlow FindById(Guid id);

        List<WorkFlow> QueryAuthorized(Guid entityid, FlowType category);

        List<WorkFlow> Query(Func<QueryDescriptor<WorkFlow>, QueryDescriptor<WorkFlow>> container);

        PagedList<WorkFlow> QueryPaged(Func<QueryDescriptor<WorkFlow>, QueryDescriptor<WorkFlow>> container);

        PagedList<WorkFlow> QueryPaged(Func<QueryDescriptor<WorkFlow>, QueryDescriptor<WorkFlow>> container, Guid solutionId, bool existInSolution);
    }
}