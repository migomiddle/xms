using PetaPoco;
using System;
using Xms.Core;

namespace Xms.Business.SerialNumber.Domain
{
    [TableName("SerialNumberRule")]
    [PrimaryKey("SerialNumberRuleId", AutoIncrement = false)]
    public class SerialNumberRule
    {
        public Guid SerialNumberRuleId { get; set; } = Guid.NewGuid();
        public Guid EntityId { get; set; }
        public Guid AttributeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Prefix { get; set; }
        public string Seprator { get; set; }
        public int DateFormatType { get; set; }
        public int IncrementLength { get; set; }
        public int Increment { get; set; }
        public RecordState StateCode { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public Guid SolutionId { get; set; }
        public int ComponentState { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid OrganizationId { get; set; }

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "AttributeId", LinkToTableName = "Attribute", LinkToFieldName = "AttributeId", TargetFieldName = "Name")]
        public string AttributeName { get; set; }

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "EntityId", LinkToTableName = "Entity", LinkToFieldName = "EntityId", TargetFieldName = "Name")]
        public string EntityName { get; set; }
    }
}