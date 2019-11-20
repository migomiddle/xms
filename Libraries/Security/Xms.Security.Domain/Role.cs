using PetaPoco;
using System;
using Xms.Core;

namespace Xms.Security.Domain
{
    [TableName("roles")]
    [PrimaryKey("roleid", AutoIncrement = false)]
    public partial class Role
    {
        public Guid RoleId { get; set; } = Guid.NewGuid();

        public string Name { get; set; }
        public string Description { get; set; }

        public RecordState StateCode { get; set; }

        public Guid OrganizationId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}