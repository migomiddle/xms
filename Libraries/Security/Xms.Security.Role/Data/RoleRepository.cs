using System;
using Xms.Core.Data;
using Xms.Data;

namespace Xms.Security.Role.Data
{
    /// <summary>
    /// 角色仓储
    /// </summary>
    public class RoleRepository : DefaultRepository<Domain.Role>, IRoleRepository
    {
        public RoleRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public string GetRolesXml(Guid solutionId)
        {
            //var result = new DataRepositoryBase<object>(DbContext).Find(@"select convert(xml,(select RoleId,Name,Description,State,BusinessUnitId,ParentRoleId
            //,CONVERT(xml,(select PrivilegeId from RolePrivileges FOR XML PATH('RolePrivilegeInfo'))) as RolePrivileges
            //,CONVERT(xml,(select EntityPermissionId,DepthMask from RoleEntityPermissions FOR XML PATH('RoleEntityPermissionInfo'))) as RoleEntityPermissions
            //from Roles
            //where exists (select 1 from SolutionComponent where SolutionId = @0 and ComponentType = @1)
            //FOR XML PATH('Role'),ROOT('Roles')))", solutionId, SolutionComponentType.Role);
            //if (result != null)
            //{
            //    var data = (result as IDictionary<string, object>).ToList();
            //    if (data.NotEmpty() && data[0].Value != null)
            //    {
            //        return data[0].Value.ToString();
            //    }
            //}
            return string.Empty;
        }
    }
}