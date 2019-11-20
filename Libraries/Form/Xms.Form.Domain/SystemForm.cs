using PetaPoco;
using System;
using Xms.Core;

namespace Xms.Form.Domain
{
    [TableName("systemform")]
    [PrimaryKey("systemformid", AutoIncrement = false)]
    public partial class SystemForm
    {
        public bool CanBeDeleted { get; set; }
        public int ComponentState { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CustomButtons { get; set; }
        public string Description { get; set; }
        public Guid EntityId { get; set; }

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "EntityId", LinkToTableName = "Entity", LinkToFieldName = "EntityId", TargetFieldName = "name")]
        public string EntityName { get; set; }

        public string FormConfig { get; set; }
        public int FormType { get; set; }
        public bool AuthorizationEnabled { get; set; }
        public bool IsCustomButton { get; set; }
        public bool IsCustomizable { get; set; }
        public bool IsDefault { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Name { get; set; }

        public Guid OrganizationId { get; set; }
        public DateTime? PublishedOn { get; set; }
        public Guid SolutionId { get; set; }
        public RecordState StateCode { get; set; }
        public Guid SystemFormId { get; set; } = Guid.NewGuid();
    }
}