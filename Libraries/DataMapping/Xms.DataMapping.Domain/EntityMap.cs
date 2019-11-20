using PetaPoco;
using System;
using System.Collections.Generic;
using Xms.Core;
using Xms.DataMapping.Abstractions;
using Xms.Schema.Domain;

namespace Xms.DataMapping.Domain
{
    [TableName("EntityMap")]
    [PrimaryKey("EntityMapId", AutoIncrement = false)]
    public class EntityMap
    {
        public Guid EntityMapId { get; set; } = Guid.NewGuid();
        public Guid TargetEntityId { get; set; }
        public Guid SourceEntityId { get; set; }
        public MapType MapType { get; set; }
        public Guid SolutionId { get; set; }
        public int ComponentState { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid ParentEntityMapId { get; set; }
        public string RelationShipName { get; set; }
        public RecordState StateCode { get; set; }

        [ResultColumn]
        [LinkEntity(typeof(Entity), LinkFromFieldName = "TargetEntityId", LinkToFieldName = "EntityId", TargetFieldName = "name", AliasName = "TargetEntity")]
        public string TargetEnttiyName { get; set; }

        [ResultColumn]
        [LinkEntity(typeof(Entity), LinkFromFieldName = "SourceEntityId", LinkToFieldName = "EntityId", TargetFieldName = "name", AliasName = "SourceEntity")]
        public string SourceEnttiyName { get; set; }

        [Ignore]
        [ResultColumn]
        public List<AttributeMap> AttributeMaps { get; set; }
    }
}