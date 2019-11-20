using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;

namespace Xms.QueryView
{
    public interface IQueryViewFinder
    {
        Domain.QueryView Find(Expression<Func<Domain.QueryView, bool>> predicate);

        Domain.QueryView FindById(Guid id);

        Domain.QueryView FindEntityDefaultView(Guid entityId);

        Domain.QueryView FindEntityDefaultView(string entityName);

        List<Domain.QueryView> FindAll();

        List<Domain.QueryView> FindByEntityId(Guid entityId);

        List<Domain.QueryView> FindByEntityName(string entityName);

        List<Domain.QueryView> Query(Func<QueryDescriptor<Domain.QueryView>, QueryDescriptor<Domain.QueryView>> container);

        List<Domain.QueryView> QueryAuthorized(Func<QueryDescriptor<Domain.QueryView>, QueryDescriptor<Domain.QueryView>> container);

        PagedList<Domain.QueryView> QueryPaged(Func<QueryDescriptor<Domain.QueryView>, QueryDescriptor<Domain.QueryView>> container);

        PagedList<Domain.QueryView> QueryPaged(Func<QueryDescriptor<Domain.QueryView>, QueryDescriptor<Domain.QueryView>> container, Guid solutionId, bool existInSolution);
    }
}