using PetaPoco;
using System;

namespace Xms.Solution.Domain
{
    [TableName("SolutionComponent")]
    [PrimaryKey("SolutionComponentId", AutoIncrement = false)]
    public class SolutionComponent
    {
        public Guid SolutionComponentId { get; set; } = Guid.NewGuid();
        public Guid SolutionId { get; set; }

        public int ComponentType { get; set; }

        public Guid ObjectId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }

        [Ignore]
        public string ComponentTypeName { get; set; }

        [Ignore]
        public string Name { get; set; }

        [Ignore]
        public string LocalizedName { get; set; }

        [Ignore]
        public string CreatedByName { get; set; }
    }
}