using PetaPoco;
using System;
using Xms.Core;

namespace Xms.Security.Domain
{
    [TableName("EntityPermission")]
    [PrimaryKey("EntityPermissionId", AutoIncrement = false)]
    public partial class EntityPermission
    {
        public Guid EntityPermissionId { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public Guid EntityId { get; set; }

        public RecordState State { get; set; }

        public AccessRightValue AccessRight { get; set; }
        public string Description { get; set; }

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "EntityId", LinkToTableName = "Entity", LinkToFieldName = "EntityId", TargetFieldName = "Name")]
        public string EntityName { get; set; }

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "EntityId", LinkToTableName = "Entity", LinkToFieldName = "EntityId", TargetFieldName = "LocalizedName")]
        public string EntityLocalizedName { get; set; }
    }
}