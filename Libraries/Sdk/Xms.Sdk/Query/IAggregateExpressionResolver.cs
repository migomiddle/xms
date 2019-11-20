using System.Collections.Generic;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Query
{
    public interface IAggregateExpressionResolver
    {
        List<dynamic> Execute(AggregateExpression agg);

        List<dynamic> Execute(QueryExpression query, Dictionary<string, AggregateType> attributeAggs, List<string> groupFields);

        List<dynamic> GroupingTop(int top, QueryExpression query, string groupField, OrderExpression order);
    }
}