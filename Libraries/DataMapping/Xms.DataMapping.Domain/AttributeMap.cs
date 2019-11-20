using PetaPoco;
using System;
using Xms.Core;

namespace Xms.DataMapping.Domain
{
    [TableName("AttributeMap")]
    [PrimaryKey("AttributeMapId", AutoIncrement = false)]
    public class AttributeMap
    {
        public Guid AttributeMapId { get; set; } = Guid.NewGuid();

        public Guid TargetAttributeId { get; set; }
        public Guid SourceAttributeId { get; set; }

        public Guid EntityMapId { get; set; }
        public bool CanChange { get; set; }

        public Guid RemainAttributeId { get; set; }
        public Guid ClosedAttributeId { get; set; }

        public string DefaultValue { get; set; }

        [ResultColumn]
        [LinkEntity(typeof(Schema.Domain.Attribute), LinkFromFieldName = "TargetAttributeId", LinkToFieldName = "AttributeId", TargetFieldName = "name", AliasName = "TargetAttribute")]
        public string TargetAttributeName { get; set; }

        [ResultColumn]
        [LinkEntity(typeof(Schema.Domain.Attribute), LinkFromFieldName = "SourceAttributeId", LinkToFieldName = "AttributeId", TargetFieldName = "name", AliasName = "SourceAttribute")]
        public string SourceAttributeName { get; set; }

        [ResultColumn]
        [LinkEntity(typeof(Schema.Domain.Attribute), LinkFromFieldName = "RemainAttributeId", LinkToFieldName = "AttributeId", TargetFieldName = "name", AliasName = "RemainAttribute")]
        public string RemainAttributeName { get; set; }

        [ResultColumn]
        [LinkEntity(typeof(Schema.Domain.Attribute), LinkFromFieldName = "ClosedAttributeId", LinkToFieldName = "AttributeId", TargetFieldName = "name", AliasName = "ClosedAttribute")]
        public string ClosedAttributeName { get; set; }
    }
}