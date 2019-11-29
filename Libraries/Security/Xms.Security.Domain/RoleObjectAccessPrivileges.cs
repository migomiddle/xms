using System;

namespace Xms.Security.Domain
{
    public class RoleObjectAccessPrivileges
    {
        public Guid RolePrivilegeId { get; set; } = Guid.NewGuid();
        public Guid RoleId { get; set; }
        public Guid PrivilegeId { get; set; }

        public string RoleName { get; set; }

        public string PrivilegeName { get; set; }
    }
}