using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.SiteMap;
using Xms.SiteMap.Domain;

namespace Xms.Security.Principal
{
    /// <summary>
    /// 菜单权限验证服务
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly ISystemUserPermissionService _systemUserPermissionService;
        private readonly ICurrentUser _user;
        private readonly IPrivilegeService _privilegeService;

        public PermissionService(ICurrentUser user, ISystemUserPermissionService systemUserPermissionService, IPrivilegeService privilegeService)
        {
            _user = user;
            _privilegeService = privilegeService;
            _systemUserPermissionService = systemUserPermissionService;
        }

        /// <summary>
        /// 权限判断
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="ignoreNull">是否忽略不存在的权限项</param>
        /// <returns></returns>
        public bool HasPermission(string areaName, string className, string methodName, bool ignoreNull = true)
        {
            //获取权限项
            var p = _privilegeService.Find(areaName, className, methodName);
            return ValidPermission(p, ignoreNull);
        }

        public bool HasPermission(string url, bool ignoreNull = true)
        {
            //获取权限项
            var p = _privilegeService.Find(url);
            return ValidPermission(p, ignoreNull);
        }

        private bool ValidPermission(Privilege p, bool ignoreNull = true)
        {
            if (ignoreNull && null == p)
            {
                return true;
            }
            if (!p.AuthorizationEnabled)
            {
                return true;
            }
            if (_user.Privileges.IsEmpty())
            {
                var up = _systemUserPermissionService.GetAuthPrivilege(_user.SystemUserId, p.Url);
                if (up != null)
                {
                    return true;
                }
            }
            else if (_user.Privileges.Find(n => n.PrivilegeId == p.PrivilegeId) != null)
            {
                return true;
            }
            return false;
        }
    }
}