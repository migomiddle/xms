using PetaPoco;
using System;

namespace Xms.Security.Domain
{
    [TableName("RoleObjectAccess")]
    [PrimaryKey("RoleObjectAccessId", AutoIncrement = false)]
    public class RoleObjectAccess
    {
        public Guid RoleObjectAccessId { get; set; } = Guid.NewGuid();
        public Guid RoleId { get; set; }
        public Guid ObjectId { get; set; }
        public int ObjectTypeCode { get; set; }
        public int AccessRightsMask { get; set; }
    }
}