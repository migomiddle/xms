using PetaPoco;
using System;
using System.Xml.Serialization;
using Xms.Schema.Abstractions;

namespace Xms.Schema.Domain
{
    [TableName("Relationship")]
    [PrimaryKey("RelationshipId", AutoIncrement = false)]
    public class RelationShip
    {
        public Guid RelationshipId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public Guid ReferencingEntityId { get; set; }
        public Guid ReferencingAttributeId { get; set; }
        public Guid ReferencedEntityId { get; set; }
        public Guid ReferencedAttributeId { get; set; }
        public RelationShipType RelationshipType { get; set; }
        public int CascadeLinkMask { get; set; }
        public int CascadeDelete { get; set; }
        public int CascadeAssign { get; set; }
        public int CascadeShare { get; set; }
        public int CascadeUnShare { get; set; }
        public bool IsCustomizable { get; set; }
        public Guid SolutionId { get; set; }
        public int ComponentState { get; set; }

        [ResultColumn]
        //[LinkEntity(typeof(EntityInfo), AliasName = "ReferencedEntity", LinkFromFieldName = "ReferencedEntityId", LinkToFieldName = "EntityId", TargetFieldName = "Name")]
        public string ReferencedEntityName { get; set; }

        [ResultColumn]
        //[LinkEntity(typeof(EntityInfo), AliasName = "ReferencedEntity", LinkFromFieldName = "ReferencedEntityId", LinkToFieldName = "EntityId", TargetFieldName = "LocalizedName")]
        public string ReferencedEntityLocalizedName { get; set; }

        [ResultColumn]
        //[LinkEntity(typeof(EntityInfo), AliasName = "ReferencedAttribute", LinkFromFieldName = "ReferencedAttributeId", LinkToFieldName = "AttributeId", TargetFieldName = "Name")]
        public string ReferencedAttributeName { get; set; }

        [ResultColumn]
        //[LinkEntity(typeof(EntityInfo), AliasName = "ReferencedAttribute", LinkFromFieldName = "ReferencedAttributeId", LinkToFieldName = "AttributeId", TargetFieldName = "AttributeTypeName")]
        public string ReferencedAttributeTypeName { get; set; }

        [ResultColumn]
        //[LinkEntity(typeof(EntityInfo), AliasName = "ReferencedAttribute", LinkFromFieldName = "ReferencedAttributeId", LinkToFieldName = "AttributeId", TargetFieldName = "LocalizedName")]
        public string ReferencedAttributeLocalizedName { get; set; }

        [ResultColumn]
        //[LinkEntity(typeof(EntityInfo), AliasName = "ReferencingEntity", LinkFromFieldName = "ReferencingEntityId", LinkToFieldName = "EntityId", TargetFieldName = "Name")]
        public string ReferencingEntityName { get; set; }

        [ResultColumn]
        //[LinkEntity(typeof(EntityInfo), AliasName = "ReferencingEntity", LinkFromFieldName = "ReferencingEntityId", LinkToFieldName = "EntityId", TargetFieldName = "LocalizedName")]
        public string ReferencingEntityLocalizedName { get; set; }

        [ResultColumn]
        //[LinkEntity(typeof(EntityInfo), AliasName = "ReferencingAttribute", LinkFromFieldName = "ReferencingAttributeId", LinkToFieldName = "AttributeId", TargetFieldName = "Name")]
        public string ReferencingAttributeName { get; set; }

        [ResultColumn]
        //[LinkEntity(typeof(EntityInfo), AliasName = "ReferencingAttribute", LinkFromFieldName = "ReferencingAttributeId", LinkToFieldName = "AttributeId", TargetFieldName = "LocalizedName")]
        public string ReferencingAttributeLocalizedName { get; set; }

        [ResultColumn]
        //[LinkEntity(typeof(EntityInfo), AliasName = "ReferencingAttribute", LinkFromFieldName = "ReferencingAttributeId", LinkToFieldName = "AttributeId", TargetFieldName = "AttributeTypeName")]
        public string ReferencingAttributeTypeName { get; set; }
    }

    public class RelationShipXmlInfo : RelationShip
    {
        [XmlIgnore]
        public new RelationShipType RelationshipType { get; set; }

        [XmlAttribute("RelationshipType")]
        public int RelationshipTypeInt
        {
            get { return (int)RelationshipType; }
            set { RelationshipType = (RelationShipType)value; }
        }
    }
}