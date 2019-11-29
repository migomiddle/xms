using Xms.Business.Filter.Domain;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;
using Xms.Schema.Attribute;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Extensions;

namespace Xms.Business.Filter
{
    public static class FilterRuleExtensions
    {
        public static bool IsTrue(this FilterRule rule, IAttributeFinder attributeFinder, Entity record)
        {
            bool flag = true;
            if (rule.Conditions.IsNotEmpty())
            {
                var ruleConditions = new FilterRuleConditions();
                ruleConditions = ruleConditions.DeserializeFromJson(rule.Conditions);
                foreach (var cnd in ruleConditions.Conditions)
                {
                    if (cnd.CompareAttributeName.IsNotEmpty())
                    {
                        cnd.Values.Add(record.GetValue<object>(cnd.CompareAttributeName));
                    }
                    var attr = attributeFinder.Find(rule.EntityId, cnd.AttributeName);
                    flag = cnd.IsTrue(attr, record.GetValue<object>(cnd.AttributeName));
                    if (ruleConditions.LogicalOperator == LogicalOperator.Or && flag)
                    {
                        break;
                    }
                    if (ruleConditions.LogicalOperator == LogicalOperator.And && !flag)
                    {
                        break;
                    }
                }
            }
            return flag;
        }
    }
}