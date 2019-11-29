using System;
using System.Collections.Generic;
using Xms.Core;
using Xms.Core.Data;
using Xms.Security.Domain;

namespace Xms.Security.DataAuthorization.Data
{
    public interface IRoleObjectAccessEntityPermissionRepository : IRepository<RoleObjectAccessEntityPermission>
    {
        RoleObjectAccessEntityPermission FindUserPermission(string entityName, string userAccountName, AccessRightValue access);

        List<RoleObjectAccessEntityPermission> GetPermissions(IEnumerable<Guid> entityIds, IEnumerable<Guid> roleIds, AccessRightValue access);
    }
}