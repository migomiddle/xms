using System;
using System.Collections.Generic;
using Xms.Core;
using Xms.DataMapping.Abstractions;
using Xms.DataMapping.Domain;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Customize.Models
{
    public class EntityMapModel : BasePaged<EntityMap>
    {
        public Guid EntityId { get; set; }
        public Guid SolutionId { get; set; }
        public bool LoadData { get; set; }
    }

    public class EditEntityMapModel
    {
        public Guid? EntityMapId { get; set; }
        public Guid EntityId { get; set; }
        public Guid SourceEntityId { get; set; }
        public MapType MapType { get; set; }
        public Guid SolutionId { get; set; }
        public Schema.Domain.Entity TargetEntityMetaData { get; set; }
        public Schema.Domain.Entity SourceEntityMetaData { get; set; }
        public List<Schema.Domain.Attribute> Attributes { get; set; }
        public EntityMap HeadEntityMap { get; set; }
        public List<AttributeMap> HeadAttributeMap { get; set; }
        public List<AttributeMap> HeadControlAttributeMap { get; set; }
        public Schema.Domain.Entity ChildTargetEntityMetaData { get; set; }
        public Schema.Domain.Entity ChildSourceEntityMetaData { get; set; }
        public Guid ChildTargetEntityId { get; set; }
        public Guid ChildSourceEntityId { get; set; }
        public RecordState StateCode { get; set; }
        public EntityMap ChildEntityMap { get; set; }
        public List<AttributeMap> ChildAttributeMap { get; set; }
        public List<AttributeMap> ChildControlAttributeMap { get; set; }
        public string RelationShipName { get; set; }
        public string ChildRelationShipName { get; set; }

        public Schema.Domain.RelationShip HeadRelationShip { get; set; }
        public Schema.Domain.RelationShip ChildRelationShip { get; set; }
    }
}