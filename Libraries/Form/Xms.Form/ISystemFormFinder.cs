using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Form.Abstractions;

namespace Xms.Form
{
    public interface ISystemFormFinder
    {
        Domain.SystemForm Find(Expression<Func<Domain.SystemForm, bool>> predicate);

        Domain.SystemForm FindById(Guid id);

        Domain.SystemForm FindEntityDefaultForm(Guid entityId);

        Domain.SystemForm FindEntityDefaultForm(string entityName);

        List<Domain.SystemForm> FindByEntityId(Guid entityId);

        List<Domain.SystemForm> FindByEntityName(string entityName);

        List<Domain.SystemForm> FindAll();

        List<Domain.SystemForm> Query(Func<QueryDescriptor<Domain.SystemForm>, QueryDescriptor<Domain.SystemForm>> container);

        PagedList<Domain.SystemForm> QueryPaged(Func<QueryDescriptor<Domain.SystemForm>, QueryDescriptor<Domain.SystemForm>> container);

        PagedList<Domain.SystemForm> QueryPaged(Func<QueryDescriptor<Domain.SystemForm>, QueryDescriptor<Domain.SystemForm>> container, Guid solutionId, bool existInSolution, FormType formType);

        List<Domain.SystemForm> QueryAuthorized(Func<QueryDescriptor<Domain.SystemForm>, QueryDescriptor<Domain.SystemForm>> container, FormType formType);
    }
}