using PetaPoco;
using System;

namespace Xms.Organization.Domain
{
    [TableName("OrganizationBase")]
    [PrimaryKey("OrganizationBaseId", AutoIncrement = false)]
    public partial class OrganizationBase
    {
        public Guid OrganizationBaseId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public int State { get; set; }
        public string UniqueName { get; set; }
        public string DataServerName { get; set; }
        public string DataAccountName { get; set; }
        public string DataPassword { get; set; }
        public string DatabaseName { get; set; }
    }
}