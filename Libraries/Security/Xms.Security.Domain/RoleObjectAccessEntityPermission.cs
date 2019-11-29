using PetaPoco;
using System;
using Xms.Core;

namespace Xms.Security.Domain
{
    [TableName("RoleObjectAccess")]
    [PrimaryKey("RoleObjectAccessId", AutoIncrement = false)]
    public class RoleObjectAccessEntityPermission
    {
        public Guid RoleObjectAccessId { get; set; } = Guid.NewGuid();
        public Guid RoleId { get; set; }
        public Guid ObjectId { get; set; }
        public int ObjectTypeCode { get; set; }
        public EntityPermissionDepth AccessRightsMask { get; set; } = EntityPermissionDepth.Self;

        [ResultColumn]
        [LinkEntity(typeof(EntityPermission), LinkFromFieldName = "RoleObjectAccessId", LinkToFieldName = "EntityPermissionId")]
        public Guid EntityId { get; set; }

        [ResultColumn]
        [LinkEntity(typeof(EntityPermission), LinkFromFieldName = "RoleObjectAccessId", LinkToFieldName = "EntityPermissionId")]
        public AccessRightValue AccessRight { get; set; }
    }
}