using PetaPoco;
using System;

namespace Xms.Business.DataAnalyse.Domain
{
    [TableName("Report")]
    [PrimaryKey("ReportId", AutoIncrement = false)]
    public class Report
    {
        public Guid ReportId { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string Description { get; set; }

        public string DefaultFilterConfig { get; set; }

        public string CustomConfig { get; set; }

        public string QueryConfig { get; set; }

        public string BodyText { get; set; }

        public string FileName { get; set; }

        public bool IsPersonal { get; set; }

        public string BodyUrl { get; set; }

        public int TypeCode { get; set; }

        public bool IsCustomizable { get; set; }

        public int ComponentState { get; set; }

        public Guid SolutionId { get; set; }

        public Guid OwnerId { get; set; }

        public int OwnerIdType { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public Guid? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public Guid EntityId { get; set; }
        public Guid RelatedEntityId { get; set; }
        public bool IsAuthorization { get; set; }
        public Guid OrganizationId { get; set; }
    }
}