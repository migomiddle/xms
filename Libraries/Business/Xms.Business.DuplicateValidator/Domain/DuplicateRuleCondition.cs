using PetaPoco;
using System;

namespace Xms.Business.DuplicateValidator.Domain
{
    [TableName("DuplicateRuleCondition")]
    [PrimaryKey("DuplicateRuleConditionId", AutoIncrement = false)]
    public class DuplicateRuleCondition
    {
        public Guid DuplicateRuleConditionId { get; set; } = Guid.NewGuid();
        public Guid DuplicateRuleId { get; set; }

        public Guid EntityId { get; set; }

        public Guid AttributeId { get; set; }

        public bool IgnoreNullValues { get; set; }
        public bool IsCaseSensitive { get; set; }
    }
}