using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xms.Sdk.Abstractions.Query
{
    public sealed class FilterExpression
    {
        private List<ConditionExpression> _conditions;
        private List<FilterExpression> _filters;

        [DataMember]
        public LogicalOperator FilterOperator { get; set; }

        [DataMember]
        public List<ConditionExpression> Conditions
        {
            get
            {
                if (this._conditions == null)
                {
                    this._conditions = new List<ConditionExpression>();
                }
                return this._conditions;
            }
            private set
            {
                this._conditions = value;
            }
        }

        [DataMember]
        public List<FilterExpression> Filters
        {
            get
            {
                if (this._filters == null)
                {
                    this._filters = new List<FilterExpression>();
                }
                return this._filters;
            }
            private set
            {
                this._filters = value;
            }
        }

        public FilterExpression()
        {
        }

        public FilterExpression(LogicalOperator filterOperator)
        {
            this.FilterOperator = filterOperator;
        }

        public FilterExpression AddCondition(string attributeName, ConditionOperator conditionOperator, params object[] values)
        {
            this.Conditions.Add(new ConditionExpression(attributeName, conditionOperator, values));
            return this;
        }

        public FilterExpression AddCondition(string attributeName, ConditionOperator conditionOperator, params System.Guid[] values)
        {
            this.Conditions.Add(new ConditionExpression(attributeName, conditionOperator, values));
            return this;
        }

        public FilterExpression AddCondition(string attributeName, ConditionOperator conditionOperator, params int[] values)
        {
            this.Conditions.Add(new ConditionExpression(attributeName, conditionOperator, values));
            return this;
        }

        public FilterExpression AddCondition(string attributeName, ConditionOperator conditionOperator, params string[] values)
        {
            this.Conditions.Add(new ConditionExpression(attributeName, conditionOperator, values));
            return this;
        }

        public FilterExpression AddCondition(string attributeName, ConditionOperator conditionOperator)
        {
            this.Conditions.Add(new ConditionExpression(attributeName, conditionOperator));
            return this;
        }

        public FilterExpression AddCondition(ConditionExpression condition)
        {
            if (condition != null)
            {
                this.Conditions.Add(condition);
            }
            return this;
        }

        public FilterExpression AddFilter(FilterExpression childFilter)
        {
            if (childFilter != null)
            {
                this.Filters.Add(childFilter);
            }
            return this;
        }
    }
}