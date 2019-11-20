using System;
using System.Collections.Generic;
using Xms.Core.Context;

namespace Xms.Security.Role
{
    public interface IRoleService
    {
        bool Create(Domain.Role entity);

        bool DeleteById(Guid id);

        bool DeleteById(IEnumerable<Guid> ids);

        Domain.Role FindById(Guid id);

        List<Domain.Role> FindAll();

        List<Domain.Role> Query(Func<QueryDescriptor<Domain.Role>, QueryDescriptor<Domain.Role>> container);

        PagedList<Domain.Role> QueryPaged(Func<QueryDescriptor<Domain.Role>, QueryDescriptor<Domain.Role>> container);

        bool Update(Func<UpdateContext<Domain.Role>, UpdateContext<Domain.Role>> context);

        bool Update(Domain.Role entity);
    }
}