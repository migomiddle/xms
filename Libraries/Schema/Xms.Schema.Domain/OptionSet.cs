using PetaPoco;
using System;
using System.Collections.Generic;

namespace Xms.Schema.Domain
{
    [TableName("OptionSet")]
    [PrimaryKey("OptionSetId", AutoIncrement = false)]
    public class OptionSet
    {
        public Guid OptionSetId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public bool IsPublic { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string Description { get; set; }

        public Guid SolutionId { get; set; }
        public int ComponentState { get; set; }
        public Guid OrganizationId { get; set; }

        [Ignore]
        public List<OptionSetDetail> Items { get; set; }
    }
}