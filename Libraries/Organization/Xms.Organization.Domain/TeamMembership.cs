using PetaPoco;
using System;

namespace Xms.Organization.Domain
{
    [TableName("TeamMembership")]
    [PrimaryKey("TeamMembershipId", AutoIncrement = false)]
    public class TeamMembership
    {
        public Guid TeamMembershipId { get; set; } = Guid.NewGuid();
        public Guid TeamId { get; set; }
        public Guid SystemUserId { get; set; }
    }
}