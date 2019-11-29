using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Core;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Security.DataAuthorization.Data;
using Xms.Security.Domain;

namespace Xms.Security.DataAuthorization
{
    /// <summary>
    /// 角色实体权限服务
    /// </summary>
    public class RoleObjectAccessEntityPermissionService : IRoleObjectAccessEntityPermissionService
    {
        private readonly IRoleObjectAccessEntityPermissionRepository _roleObjectAccessEntityPermissionRepository;

        public PagedList<RoleObjectAccessEntityPermission> QueryPaged(Func<QueryDescriptor<RoleObjectAccessEntityPermission>, QueryDescriptor<RoleObjectAccessEntityPermission>> container)
        {
            QueryDescriptor<RoleObjectAccessEntityPermission> q = container(QueryDescriptorBuilder.Build<RoleObjectAccessEntityPermission>());

            return _roleObjectAccessEntityPermissionRepository.QueryPaged(q);
        }

        public List<RoleObjectAccessEntityPermission> Query(Func<QueryDescriptor<RoleObjectAccessEntityPermission>, QueryDescriptor<RoleObjectAccessEntityPermission>> container)
        {
            QueryDescriptor<RoleObjectAccessEntityPermission> q = container(QueryDescriptorBuilder.Build<RoleObjectAccessEntityPermission>());

            return _roleObjectAccessEntityPermissionRepository.Query(q)?.ToList();
        }

        public RoleObjectAccessEntityPermissionService(IRoleObjectAccessEntityPermissionRepository roleObjectAccessEntityPermissionRepository)
        {
            _roleObjectAccessEntityPermissionRepository = roleObjectAccessEntityPermissionRepository;
        }

        public RoleObjectAccessEntityPermission FindUserPermission(string entityName, string userAccountName, AccessRightValue access)
        {
            return _roleObjectAccessEntityPermissionRepository.FindUserPermission(entityName, userAccountName, access);
        }

        public List<RoleObjectAccessEntityPermission> GetPermissions(IEnumerable<Guid> entityIds, IEnumerable<Guid> roleIds, AccessRightValue access)
        {
            return _roleObjectAccessEntityPermissionRepository.GetPermissions(entityIds, roleIds, access);
        }
    }
}