using PetaPoco;
using System;
using System.Collections.Generic;
using Xms.Core;
using Xms.Core.Data;
using Xms.Data;
using Xms.Security.Domain;

namespace Xms.Security.DataAuthorization.Data
{
    /// <summary>
    /// 角色授权数据仓储
    /// </summary>
    public class RoleObjectAccessEntityPermissionRepository : DefaultRepository<RoleObjectAccessEntityPermission>, IRoleObjectAccessEntityPermissionRepository
    {
        public RoleObjectAccessEntityPermissionRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public RoleObjectAccessEntityPermission FindUserPermission(string entityName, string userAccountName, AccessRightValue access)
        {
            Sql s = Sql.Builder.Append(@"SELECT a.AccessRight,MAX(b.AccessRightsMask) AccessRightsMask FROM EntityPermission a
                INNER JOIN RoleObjectAccess b ON a.EntityPermissionId = b.ObjectId
                INNER JOIN Entity c ON a.EntityId = c.EntityId AND c.AuthorizationEnabled=1 AND c.Name=@0
                INNER JOIN SystemUserRoles d ON b.RoleId = d.RoleId
                INNER JOIN SystemUser e ON d.SystemUserId = e.SystemUserId AND e.LoginName=@1
                WHERE a.AccessRight=@2
                GROUP BY a.AccessRight", entityName, userAccountName, access);

            return _repository.Find(s.SQL, s.Arguments);
        }

        public List<RoleObjectAccessEntityPermission> GetPermissions(IEnumerable<Guid> entityIds, IEnumerable<Guid> roleIds, AccessRightValue access)
        {
            Sql s = Sql.Builder.Append(@"SELECT MAX([RoleObjectAccess].[AccessRightsMask]) AS AccessRightsMask,[EntityPermission].[EntityId] AS EntityId,[EntityPermission].[AccessRight] AS AccessRight
                       FROM RoleObjectAccess WITH(NOLOCK)
                       INNER JOIN EntityPermission AS EntityPermission WITH(NOLOCK) ON EntityPermission.[EntityPermissionId]=RoleObjectAccess.[ObjectId]
                       WHERE [EntityPermission].[EntityId] IN (@0)
                       AND [RoleObjectAccess].[RoleId] IN (@1)
                       AND [EntityPermission].[AccessRight] = @2
                       GROUP BY [EntityPermission].[EntityId],[EntityPermission].[AccessRight]", entityIds, roleIds, (int)access);

            return _repository.ExecuteQuery(s.SQL, s.Arguments);
        }
    }
}