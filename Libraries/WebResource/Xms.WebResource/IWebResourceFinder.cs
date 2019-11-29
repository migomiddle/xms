using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;

namespace Xms.WebResource
{
    public interface IWebResourceFinder
    {
        Domain.WebResource Find(Expression<Func<Domain.WebResource, bool>> predicate);

        Domain.WebResource FindById(Guid id, bool IsPublished = true);

        List<Domain.WebResource> FindByIds(params Guid[] ids);

        List<Domain.WebResource> Query(Func<QueryDescriptor<Domain.WebResource>, QueryDescriptor<Domain.WebResource>> container);

        PagedList<Domain.WebResource> QueryPaged(Func<QueryDescriptor<Domain.WebResource>, QueryDescriptor<Domain.WebResource>> container);

        PagedList<Domain.WebResource> QueryPaged(Func<QueryDescriptor<Domain.WebResource>, QueryDescriptor<Domain.WebResource>> container, Guid solutionId, bool existInSolution);
    }
}