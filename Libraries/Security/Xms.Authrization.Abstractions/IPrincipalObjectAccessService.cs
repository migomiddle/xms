using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Security.Domain;

namespace Xms.Authorization.Abstractions
{
    public interface IPrincipalObjectAccessService
    {
        bool Create(PrincipalObjectAccess entity);

        bool CreateMany(IEnumerable<PrincipalObjectAccess> entities);

        bool DeleteById(Guid id);

        bool DeleteById(IEnumerable<Guid> ids);

        bool DeleteByObjectId(Guid entityId, Guid objectId);

        PrincipalObjectAccess Find(Expression<Func<PrincipalObjectAccess, bool>> predicate);

        PrincipalObjectAccess FindById(Guid id);

        List<PrincipalObjectAccess> Query(Func<QueryDescriptor<PrincipalObjectAccess>, QueryDescriptor<PrincipalObjectAccess>> container);

        PagedList<PrincipalObjectAccess> QueryPaged(Func<QueryDescriptor<PrincipalObjectAccess>, QueryDescriptor<PrincipalObjectAccess>> container);

        bool Update(Func<UpdateContext<PrincipalObjectAccess>, UpdateContext<PrincipalObjectAccess>> context);

        bool Update(PrincipalObjectAccess entity);
    }
}