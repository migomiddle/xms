using PetaPoco;
using System;
using System.Collections.Generic;
using Xms.Core;

namespace Xms.Business.DuplicateValidator.Domain
{
    [TableName("DuplicateRule")]
    [PrimaryKey("DuplicateRuleId", AutoIncrement = false)]
    public class DuplicateRule
    {
        public Guid DuplicateRuleId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }

        public bool Intercepted { get; set; }
        public Guid EntityId { get; set; }

        public RecordState StateCode { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public Guid SolutionId { get; set; }
        public int ComponentState { get; set; }

        [Ignore]
        [ResultColumn]
        public List<DuplicateRuleCondition> Conditions { get; set; }
    }
}