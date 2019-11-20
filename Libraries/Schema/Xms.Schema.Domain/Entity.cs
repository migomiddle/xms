using PetaPoco;
using System;
using System.Collections.Generic;
using Xms.Schema.Abstractions;

namespace Xms.Schema.Domain
{
    [TableName("Entity")]
    [PrimaryKey("EntityId", AutoIncrement = false)]
    public class Entity
    {
        public Guid EntityId { get; set; } = Guid.NewGuid();
        public Guid? ParentEntityId { get; set; }
        public string Name { get; set; }
        public bool LogEnabled { get; set; }
        public bool IsCustomizable { get; set; }
        public string LocalizedName { get; set; }
        public string Description { get; set; }
        public EntityMaskEnum EntityMask { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public bool AuthorizationEnabled { get; set; }
        public bool DuplicateEnabled { get; set; }
        public bool WorkFlowEnabled { get; set; }
        public bool BusinessFlowEnabled { get; set; }
        public Guid SolutionId { get; set; }
        public int ComponentState { get; set; }
        public Guid OrganizationId { get; set; }
        public string EntityGroups { get; set; }

        [ResultColumn]
        public int ObjectTypeCode { get; set; }

        [Ignore]
        public List<Attribute> Attributes { get; set; }
    }
}