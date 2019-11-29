using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Security.Domain;

namespace Xms.Authorization.Abstractions
{
    public interface IRoleObjectAccessService
    {
        List<Guid> Authorized(string objectTypeName, params Guid[] objectId);

        bool Create(RoleObjectAccess entity);

        bool CreateMany(IEnumerable<RoleObjectAccess> entities);

        bool CreateMany(string objectTypeName, Guid objectId, params Guid[] roleId);

        bool DeleteById(Guid id);

        bool DeleteById(IEnumerable<Guid> ids);

        bool DeleteByObjectId(Guid objectId, int objectTypeCode);

        bool DeleteByObjectId(Guid objectId, string objectTypeName);

        bool DeleteByObjectId(int objectTypeCode, params Guid[] objectId);

        bool DeleteByObjectId(string objectTypeName, params Guid[] objectId);

        bool Exists(Guid objectId, int objectTypeCode, params Guid[] roleId);

        bool Exists(Guid objectId, string objectTypeName, params Guid[] roleId);

        bool DeleteByRole(Guid roleId, int objectTypeCode);

        bool DeleteByRole(Guid roleId, string objectTypeName);

        RoleObjectAccess Find(Expression<Func<RoleObjectAccess, bool>> predicate);

        RoleObjectAccess FindById(Guid id);

        List<RoleObjectAccess> Query(Func<QueryDescriptor<RoleObjectAccess>, QueryDescriptor<RoleObjectAccess>> container);

        List<RoleObjectAccess> Query(Guid objectId, string objectTypeName);

        List<RoleObjectAccess> QueryRolePermissions(Guid roleId, string objectTypeName);

        PagedList<RoleObjectAccess> QueryPaged(Func<QueryDescriptor<RoleObjectAccess>, QueryDescriptor<RoleObjectAccess>> container);

        bool Update(Func<UpdateContext<RoleObjectAccess>, UpdateContext<RoleObjectAccess>> context);

        bool Update(RoleObjectAccess entity);
    }
}