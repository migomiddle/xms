using PetaPoco;
using System;
using Xms.Core;

namespace Xms.Organization.Domain
{
    [TableName("systemuser")]
    [PrimaryKey("SystemUserId", AutoIncrement = false)]
    public partial class SystemUser
    {
        public Guid SystemUserId { get; set; } = Guid.NewGuid();
        public string LoginName { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string Avator { get; set; }
        public string Salt { get; set; }
        public int Gender { get; set; }
        public Guid BusinessUnitId { get; set; }
        public Guid SystemUserTypeId { get; set; }
        public RecordState StateCode { get; set; }
        public bool IsDeleted { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastLoginTime { get; set; }
        public DateTime? LastVisitTime { get; set; }
        public Guid OrganizationId { get; set; }

        [ResultColumn]
        [LinkEntity(typeof(Organization), LinkFromFieldName = "OrganizationId", LinkToFieldName = "OrganizationId", TargetFieldName = "UniqueName")]
        public string UniqueName { get; set; }

        public string DeviceId { get; set; }
    }
}