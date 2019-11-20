using System.Collections.Generic;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Client
{
    public interface IAggregateService
    {
        List<dynamic> Execute(AggregateExpression agg);

        List<dynamic> Execute(QueryExpression query, Dictionary<string, AggregateType> attributeAggs, List<string> groupFields);

        List<dynamic> GroupingTop(int top, string groupField, QueryExpression query, OrderExpression order);
    }
}