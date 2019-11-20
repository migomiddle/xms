using PetaPoco;
using System;
using Xms.Core;

namespace Xms.Security.Domain
{
    [Serializable]
    [TableName("SystemUserRoles")]
    [PrimaryKey("SystemUserRoleId", AutoIncrement = false)]
    public partial class SystemUserRoles
    {
        public Guid SystemUserRoleId { get; set; } = Guid.NewGuid();
        public Guid SystemUserId { get; set; }

        public Guid RoleId { get; set; }

        [ResultColumn]
        [LinkEntity(typeof(Role), LinkFromFieldName = "RoleId", LinkToFieldName = "RoleId", TargetFieldName = "Name")]
        public string RoleName { get; set; }

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "SystemUserId", LinkToTableName = "SystemUser", LinkToFieldName = "SystemUserId", TargetFieldName = "Name")]
        public string UserName { get; set; }
    }
}