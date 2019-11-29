using System.Collections.Generic;
using Xms.Core.Context;
using Xms.Organization.Domain;
using Xms.Security.Domain;

namespace Xms.Identity
{
    public interface ICurrentUser : IUserContext
    {
        List<RoleObjectAccessEntityPermission> RoleObjectAccessEntityPermission { get; set; }
        Organization.Domain.Organization OrgInfo { get; set; }
        List<RoleObjectAccessPrivileges> Privileges { get; set; }
        List<SystemUserRoles> Roles { get; set; }
        string SessionId { get; set; }
        UserSettings UserSettings { get; set; }

        bool Equals(ICurrentUser u);

        bool HasValue();
    }
}