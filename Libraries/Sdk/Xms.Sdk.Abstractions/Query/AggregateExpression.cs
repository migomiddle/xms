using System.Collections.Generic;

namespace Xms.Sdk.Abstractions.Query
{
    /// <summary>
    /// 统计表达式对象
    /// </summary>
    public class AggregateExpression : QueryExpression
    {
        private List<AggregateExpressionField> _aggregateFields;

        public List<AggregateExpressionField> AggregateFields
        {
            get
            {
                if (this._aggregateFields == null)
                {
                    this._aggregateFields = new List<AggregateExpressionField>();
                }
                return this._aggregateFields;
            }
            set
            {
                this._aggregateFields = value;
            }
        }
    }

    public sealed class AggregateExpressionField
    {
        public string AttributeName { get; set; }

        public AggregateType AggregateType { get; set; }
    }
}