using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Xms.Infrastructure;

namespace Xms.Sdk.Abstractions.Query
{
    public sealed class ConditionExpression
    {
        private string _attributeName;
        private string _compareAttributeName;
        private ConditionOperator _conditionOperator;
        private List<object> _values;

        [DataMember]
        public string AttributeName
        {
            get
            {
                return this._attributeName;
            }
            set
            {
                this._attributeName = value;
            }
        }

        [DataMember]
        public string CompareAttributeName
        {
            get
            {
                return this._compareAttributeName;
            }
            set
            {
                this._compareAttributeName = value;
            }
        }

        [DataMember]
        public ConditionOperator Operator
        {
            get
            {
                return this._conditionOperator;
            }
            set
            {
                this._conditionOperator = value;
            }
        }

        [DataMember]
        public List<object> Values
        {
            get
            {
                if (this._values == null)
                {
                    this._values = new List<object>();
                }
                return this._values;
            }
            private set
            {
                this._values = value;
            }
        }

        public ConditionExpression()
        {
        }

        public ConditionExpression(string attributeName, ConditionOperator conditionOperator, params object[] values)
        {
            Guard.NotEmpty(attributeName, "attributeName");
            this._attributeName = attributeName;
            this._conditionOperator = conditionOperator;
            if (values != null)
            {
                this._values = new List<object>(values);
            }
        }

        public ConditionExpression(string attributeName, ConditionOperator conditionOperator, object value) : this(attributeName, conditionOperator, new object[]
        {
            value
        })
        {
        }

        public ConditionExpression(string attributeName, ConditionOperator conditionOperator) : this(attributeName, conditionOperator, new object[0])
        {
        }

        public ConditionExpression(string attributeName, ConditionOperator conditionOperator, ICollection values)
        {
            Guard.NotEmpty(attributeName, "attributeName");
            this._attributeName = attributeName;
            this._conditionOperator = conditionOperator;
            if (values != null)
            {
                this._values = new List<object>();
                foreach (object current in values)
                {
                    this._values.Add(current);
                }
            }
        }
    }
}