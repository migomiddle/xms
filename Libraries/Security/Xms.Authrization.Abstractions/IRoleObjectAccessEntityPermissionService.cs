using System;
using System.Collections.Generic;
using Xms.Core;
using Xms.Core.Context;
using Xms.Security.Domain;

namespace Xms.Authorization.Abstractions
{
    public interface IRoleObjectAccessEntityPermissionService
    {
        List<RoleObjectAccessEntityPermission> Query(Func<QueryDescriptor<RoleObjectAccessEntityPermission>, QueryDescriptor<RoleObjectAccessEntityPermission>> container);

        PagedList<RoleObjectAccessEntityPermission> QueryPaged(Func<QueryDescriptor<RoleObjectAccessEntityPermission>, QueryDescriptor<RoleObjectAccessEntityPermission>> container);

        RoleObjectAccessEntityPermission FindUserPermission(string entityName, string userAccountName, AccessRightValue access);

        List<RoleObjectAccessEntityPermission> GetPermissions(IEnumerable<Guid> entityIds, IEnumerable<Guid> roleIds, AccessRightValue access);
    }
}