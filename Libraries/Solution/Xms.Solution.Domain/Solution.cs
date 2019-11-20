using PetaPoco;
using System;
using Xms.Core;

namespace Xms.Solution.Domain
{
    [TableName("Solution")]
    [PrimaryKey("SolutionId", AutoIncrement = false)]
    public class Solution
    {
        public Guid SolutionId { get; set; } = Guid.NewGuid();

        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string Version { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }

        public DateTime InstalledOn { get; set; }

        public Guid PublisherId { get; set; }

        public bool IsSystem { get; set; }

        public bool IsManaged { get; set; }

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "PublisherId", LinkToTableName = "SystemUser", LinkToFieldName = "SystemUserId", TargetFieldName = "Name", AliasName = "Publisher")]
        public string PublisherIdName { get; set; }

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "CreatedBy", LinkToTableName = "SystemUser", LinkToFieldName = "SystemUserId", TargetFieldName = "Name", AliasName = "Created")]
        public string CreatedByName { get; set; }
    }
}