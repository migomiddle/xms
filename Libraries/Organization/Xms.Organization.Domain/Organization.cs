using PetaPoco;
using System;
using Xms.Localization.Abstractions;

namespace Xms.Organization.Domain
{
    [TableName("Organization")]
    [PrimaryKey("OrganizationId", AutoIncrement = false)]
    public partial class Organization
    {
        public Guid OrganizationId { get; set; } = Guid.NewGuid();

        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public LanguageCode LanguageId { get; set; }

        public Guid BaseCurrencyId { get; set; }
        public string UniqueName { get; set; }
        public string DataServerName { get; set; }
        public string DataAccountName { get; set; }
        public string DataPassword { get; set; }
        public string DatabaseName { get; set; }
        public int StateCode { get; set; }
        public int StatusCode { get; set; }
    }
}