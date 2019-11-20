using System;
using System.Collections.Generic;
using Xms.Module.Core;
using Xms.Schema.Abstractions;
using Xms.Security.Principal.Data;
using Xms.SiteMap;
using Xms.SiteMap.Domain;

namespace Xms.Security.Principal
{
    /// <summary>
    /// 用户权限服务
    /// </summary>
    public class SystemUserPermissionService : ISystemUserPermissionService
    {
        private readonly ISystemUserPermissionRepository _systemUserPermissionRepository;

        public SystemUserPermissionService(ISystemUserPermissionRepository systemUserPermissionRepository)
        {
            _systemUserPermissionRepository = systemUserPermissionRepository;
        }

        /// <summary>
        /// 获取用户菜单权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Privilege> GetPrivileges(Guid systemUserId)
        {
            return _systemUserPermissionRepository.GetPrivileges(systemUserId, Module.Core.ModuleCollection.GetIdentity(SiteMap.SiteMapDefaults.ModuleName));
        }

        /// <summary>
        /// 获取用户菜单权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="areaName"></param>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public Privilege GetAuthPrivilege(Guid systemUserId, string areaName, string className, string methodName)
        {
            return _systemUserPermissionRepository.GetAuthPrivilege(systemUserId, areaName, className, methodName, ModuleCollection.GetIdentity(SiteMapDefaults.ModuleName));
        }

        public Privilege GetAuthPrivilege(Guid systemUserId, string url)
        {
            return _systemUserPermissionRepository.GetAuthPrivilege(systemUserId, url, ModuleCollection.GetIdentity(SiteMapDefaults.ModuleName));
        }

        public List<Guid> GetNoneReadFields(Guid systemUserId, List<Guid> securityFields)
        {
            return _systemUserPermissionRepository.GetNoneReadFields(systemUserId, securityFields, ModuleCollection.GetIdentity(AttributeDefaults.ModuleName));
        }

        public List<Guid> GetNoneEditFields(Guid systemUserId, List<Guid> securityFields)
        {
            return _systemUserPermissionRepository.GetNoneEditFields(systemUserId, securityFields, ModuleCollection.GetIdentity(AttributeDefaults.ModuleName));
        }
    }
}