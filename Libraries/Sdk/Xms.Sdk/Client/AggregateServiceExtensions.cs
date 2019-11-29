using System.Collections.Generic;
using System.Linq;
using Xms.Infrastructure.Utility;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 数据统计扩展方法
    /// </summary>
    public static class AggregateServiceExtensions
    {
        public static long Count(this IAggregateService aggregateService, string entityName, FilterExpression filter)
        {
            var value = GetValue(aggregateService, entityName, filter, AggregateType.Count);
            if (value != null)
            {
                return long.Parse(value.ToString());
            }
            return 0;
        }

        public static double Sum(this IAggregateService aggregateService, string entityName, FilterExpression filter)
        {
            var value = GetValue(aggregateService, entityName, filter, AggregateType.Sum);
            if (value != null)
            {
                return double.Parse(value.ToString());
            }
            return 0;
        }

        public static double Avg(this IAggregateService aggregateService, string entityName, FilterExpression filter)
        {
            var value = GetValue(aggregateService, entityName, filter, AggregateType.Avg);
            if (value != null)
            {
                return double.Parse(value.ToString());
            }
            return 0;
        }

        public static double Max(this IAggregateService aggregateService, string entityName, FilterExpression filter)
        {
            var value = GetValue(aggregateService, entityName, filter, AggregateType.Max);
            if (value != null)
            {
                return double.Parse(value.ToString());
            }
            return 0;
        }

        public static double Min(this IAggregateService aggregateService, string entityName, FilterExpression filter)
        {
            var value = GetValue(aggregateService, entityName, filter, AggregateType.Min);
            if (value != null)
            {
                return double.Parse(value.ToString());
            }
            return 0;
        }

        public static object GetValue(this IAggregateService aggregateService, string entityName, FilterExpression filter, AggregateType aggregateType)
        {
            var agg = new AggregateExpression
            {
                EntityName = entityName
                ,
                AggregateFields = new List<AggregateExpressionField> {
                                    new AggregateExpressionField{AttributeName = entityName + "Id", AggregateType = aggregateType}
                                }
                ,
                Criteria = filter
            };
            agg.AddColumns(entityName + "Id");
            var values = aggregateService.Execute(agg);
            if (values.NotEmpty())
            {
                return (values[0] as IDictionary<string, object>).Values.First();
            }
            return null;
        }
    }
}