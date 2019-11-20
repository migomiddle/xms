using PetaPoco;
using System;
using Xms.Core;

namespace Xms.Security.Domain
{
    [TableName("PrincipalObjectAccess")]
    [PrimaryKey("PrincipalObjectAccessId", AutoIncrement = false)]
    public class PrincipalObjectAccess
    {
        public Guid PrincipalObjectAccessId { get; set; } = Guid.NewGuid();
        public Guid EntityId { get; set; }
        public Guid ObjectId { get; set; }
        public Guid PrincipalId { get; set; }
        public AccessRightValue AccessRightsMask { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [ResultColumn]
        [LinkEntity(LinkFromFieldName = "PrincipalId", LinkToTableName = "SystemUser", LinkToFieldName = "SystemUserId", TargetFieldName = "Name")]
        public string PrincipalUserName { get; set; }
    }
}