using PetaPoco;
using System;

namespace Xms.UserPersonalization.Domain
{
    [TableName("UserCustomization")]
    [PrimaryKey("UserCustomizationId", AutoIncrement = false)]
    public class UserCustomization
    {
        public Guid UserCustomizationId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public Guid OwnerId { get; set; }
        public int OwnerIdType { get; set; } = 1;
    }
}