using PetaPoco;
using System;
using Xms.Core;

namespace Xms.SiteMap.Domain
{
    [TableName("privileges")]
    [PrimaryKey("PrivilegeId", AutoIncrement = false)]
    public partial class Privilege
    {
        public Guid PrivilegeId { get; set; } = Guid.NewGuid();

        public string DisplayName { get; set; }

        public string SystemName { get; set; }

        public string ClassName { get; set; }
        public string MethodName { get; set; }

        public Guid ParentPrivilegeId { get; set; }

        public string Url { get; set; }

        public string OpenTarget { get; set; }

        public int DisplayOrder { get; set; }

        public bool AuthorizationEnabled { get; set; }

        public bool IsVisibled { get; set; }

        public string Description { get; set; }

        public string SmallIcon { get; set; }

        public string BigIcon { get; set; }

        public int Level { get; set; }
        public Guid OrganizationId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [ResultColumn]
        [LinkEntity(typeof(Privilege), AliasName = "ParentPrivilege", LinkFromFieldName = "ParentPrivilegeId", LinkToFieldName = "PrivilegeId", TargetFieldName = "DisplayName")]
        public string ParentPrivilegeName { get; set; }
    }
}