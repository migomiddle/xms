using PetaPoco;
using System;
using Xms.Core;

namespace Xms.Organization.Domain
{
    [TableName("BusinessUnit")]
    [PrimaryKey("BusinessUnitId", AutoIncrement = false)]
    public partial class BusinessUnit
    {
        public Guid BusinessUnitId { get; set; } = Guid.NewGuid();
        public string UnitNumber { get; set; }
        public string Name { get; set; }
        public Guid? ParentBusinessUnitId { get; set; }
        public Guid OrganizationId { get; set; }
        public string Description { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public int StateCode { get; set; }

        [ResultColumn]
        [LinkEntity(typeof(BusinessUnit), AliasName = "ParentBusinessUnit", LinkFromFieldName = "ParentBusinessUnitId", LinkToFieldName = "BusinessUnitId", TargetFieldName = "name")]
        public string ParentBusinessUnitName { get; set; }

        [ResultColumn]
        [LinkEntity(typeof(Organization), LinkFromFieldName = "OrganizationId", LinkToFieldName = "OrganizationId", TargetFieldName = "name")]
        public string OrganizationName { get; set; }
    }
}