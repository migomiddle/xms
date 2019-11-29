using System.Collections.Generic;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Business.Filter
{
    public class FilterRuleConditions
    {
        public LogicalOperator LogicalOperator { get; set; }
        public List<ConditionExpression> Conditions { get; set; }
    }
}