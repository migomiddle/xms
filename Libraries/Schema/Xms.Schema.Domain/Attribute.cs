using PetaPoco;
using System;
using System.Collections.Generic;
using Xms.Core;

namespace Xms.Schema.Domain
{
    [TableName("Attribute")]
    [PrimaryKey("AttributeId", AutoIncrement = false)]
    public class Attribute
    {
        public Guid AttributeId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string LocalizedName { get; set; }
        public int MaxLength { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public string DataFormat { get; set; }
        public string DisplayStyle { get; set; }
        public int Precision { get; set; }
        public bool IsNullable { get; set; }
        public Guid EntityId { get; set; }
        public string DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public bool LogEnabled { get; set; }
        public bool IsCustomField { get; set; }
        public Guid? ReferencedEntityId { get; set; }
        public Guid? OptionSetId { get; set; }
        public bool IsCustomizable { get; set; }
        public bool AuthorizationEnabled { get; set; }
        public bool IsPrimaryField { get; set; }
        public string FormulaExpression { get; set; }
        public int ValueType { get; set; }
        public Guid SummaryEntityId { get; set; }
        public string SummaryExpression { get; set; }
        public string AttributeTypeName { get; set; }
        public string Description { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [ResultColumn]
        [LinkEntity(typeof(Entity), LinkFromFieldName = "EntityId", LinkToFieldName = "EntityId", TargetFieldName = "LocalizedName")]
        public string EntityLocalizedName { get; set; }

        [ResultColumn]
        [LinkEntity(typeof(Entity), LinkFromFieldName = "EntityId", LinkToFieldName = "EntityId", TargetFieldName = "Name")]
        public string EntityName { get; set; }

        [ResultColumn]
        [LinkEntity(typeof(Entity), LinkFromFieldName = "ReferencedEntityId", LinkToFieldName = "EntityId", TargetFieldName = "Name", AliasName = "ReferencedEntity")]
        public string ReferencedEntityName { get; set; }

        [Ignore]
        public OptionSet OptionSet { get; set; }

        [Ignore]
        public List<Domain.StringMap> PickLists { get; set; }
    }
}