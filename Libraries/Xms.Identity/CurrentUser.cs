using System;
using System.Collections.Generic;
using Xms.Organization.Data;
using Xms.Organization.Domain;
using Xms.Security.Domain;

namespace Xms.Identity
{
    /// <summary>
    /// 当前用户信息
    /// </summary>
    [Serializable]
    public class CurrentUser : ICurrentUser
    {
        private readonly ISystemUserSettingsRepository _systemUserSettingsRepository;

        public CurrentUser(ISystemUserSettingsRepository systemUserSettingsRepository)
        {
            _systemUserSettingsRepository = systemUserSettingsRepository;
        }

        public const string SESSION_KEY = "SESSION_USER";
        public Guid SystemUserId { get; set; }
        public string SessionId { get; set; }
        public string UserName { get; set; }
        public string LoginName { get; set; }
        public bool IsSuperAdmin { get; set; }
        public Guid BusinessUnitId { get; set; }
        public string BusinessUnitIdName { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public List<RoleObjectAccessPrivileges> Privileges { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public List<SystemUserRoles> Roles { get; set; }

        public Guid OrganizationId { get; set; }

        //[Newtonsoft.Json.JsonIgnore]
        public Organization.Domain.Organization OrgInfo { get; set; }

        private UserSettings _userSettings;

        private List<RoleObjectAccessEntityPermission> _roleObjectAccessEntityPermissions;

        [Newtonsoft.Json.JsonIgnore]
        public List<RoleObjectAccessEntityPermission> RoleObjectAccessEntityPermission
        {
            get
            {
                if (_roleObjectAccessEntityPermissions == null)
                {
                }
                return _roleObjectAccessEntityPermissions;
            }
            set { _roleObjectAccessEntityPermissions = value; }
        }

        //[Newtonsoft.Json.JsonIgnore]
        public UserSettings UserSettings
        {
            get
            {
                if (HasValue() && _userSettings == null)
                {
                    _userSettings = _systemUserSettingsRepository.FindById(SystemUserId);
                }
                return _userSettings ?? new UserSettings();
            }
            set { _userSettings = value; }
        }

        public bool HasValue()
        {
            return !SystemUserId.Equals(Guid.Empty);
        }

        public bool Equals(ICurrentUser u)
        {
            return SystemUserId.Equals(u.SystemUserId);
        }
    }
}