using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Security.Role
{
    /// <summary>
    /// 安全角色导入服务
    /// </summary>
    [SolutionImportNode("roles")]
    public class RoleImporter : ISolutionComponentImporter<Domain.Role>
    {
        private readonly IRoleService _roleService;

        //private readonly IRolePrivilegesService _rolePrivilegesService;
        //private readonly IRoleEntityPermissionsService _roleEntityPermissionsService;
        private readonly IAppContext _appContext;

        private readonly ICurrentUser _currentUser;

        public RoleImporter(IAppContext appContext
            , IRoleService roleService
            //, IRolePrivilegesService rolePrivilegesService
            //, IRoleEntityPermissionsService roleEntityPermissionsService
            )
        {
            _appContext = appContext;
            _currentUser = _appContext.GetFeature<ICurrentUser>();
            _roleService = roleService;
            //_rolePrivilegesService = rolePrivilegesService;
            //_roleEntityPermissionsService = roleEntityPermissionsService;
        }

        public bool Import(Guid solutionId, IList<Domain.Role> roles)
        {
            if (roles.NotEmpty())
            {
                foreach (var item in roles)
                {
                    var entity = _roleService.FindById(item.RoleId);
                    if (entity != null)
                    {
                        entity.StateCode = item.StateCode;
                        entity.Description = item.Description;
                        entity.Name = item.Name;
                        _roleService.Update(entity);
                    }
                    else
                    {
                        item.CreatedBy = _currentUser.SystemUserId;
                        item.OrganizationId = _appContext.OrganizationId;
                        _roleService.Create(item);
                    }
                    //var rolePrivileges = item.RolePrivileges.Select(n => n.PrivilegeId).ToList();
                    //if (rolePrivileges.NotEmpty())
                    //{
                    //    _rolePrivilegesService.CreateRolePrivileges(item.RoleId, rolePrivileges);
                    //}
                    //if (item.RoleEntityPermissions.NotEmpty())
                    //{
                    //    _roleEntityPermissionsService.CreateMany(item.RoleEntityPermissions);
                    //}
                }
            }
            return true;
        }
    }
}