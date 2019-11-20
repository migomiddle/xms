using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Security.Domain;

namespace Xms.Security.Principal
{
    public interface ISystemUserRolesService
    {
        bool Create(SystemUserRoles entity);

        bool CreateMany(List<SystemUserRoles> entities);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        bool DeleteById(Guid userId, Guid roleid);

        SystemUserRoles Find(Expression<Func<SystemUserRoles, bool>> predicate);

        SystemUserRoles FindById(Guid id);

        List<SystemUserRoles> FindByUserId(Guid systemUserId);

        List<SystemUserRoles> Query(Func<QueryDescriptor<SystemUserRoles>, QueryDescriptor<SystemUserRoles>> container);

        PagedList<SystemUserRoles> QueryPaged(Func<QueryDescriptor<SystemUserRoles>, QueryDescriptor<SystemUserRoles>> container);

        bool Update(Func<UpdateContext<SystemUserRoles>, UpdateContext<SystemUserRoles>> context);

        bool Update(SystemUserRoles entity);

        bool UpdateUserRoles(Guid systemUserId, List<SystemUserRoles> roles);

        bool IsAdministrator(Guid systemUserId);
    }
}