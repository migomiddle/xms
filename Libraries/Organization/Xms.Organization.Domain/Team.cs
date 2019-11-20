using PetaPoco;
using System;

namespace Xms.Organization.Domain
{
    [TableName("Team")]
    [PrimaryKey("TeamId", AutoIncrement = false)]
    public class Team
    {
        public Guid TeamId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }
}