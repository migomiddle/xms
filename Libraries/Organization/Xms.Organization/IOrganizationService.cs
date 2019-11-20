using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;

namespace Xms.Organization
{
    public interface IOrganizationService
    {
        bool Create(Domain.Organization entity);

        bool DeleteById(Guid id);

        bool DeleteById(IEnumerable<Guid> ids);

        Domain.Organization Find(Expression<Func<Domain.Organization, bool>> predicate);

        Domain.Organization FindById(Guid id);

        List<Domain.Organization> Query(Func<QueryDescriptor<Domain.Organization>, QueryDescriptor<Domain.Organization>> container);

        PagedList<Domain.Organization> QueryPaged(Func<QueryDescriptor<Domain.Organization>, QueryDescriptor<Domain.Organization>> container);

        bool Update(Func<UpdateContext<Domain.Organization>, UpdateContext<Domain.Organization>> context);

        bool Update(Domain.Organization entity);
    }
}