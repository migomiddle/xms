using System;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Data
{
    public sealed class DataFieldExpressionHelper
    {
        public static string GetDateGroupingExpression(DateGroupingType dgt, string field)
        {
            switch (dgt)
            {
                case DateGroupingType.Year:
                    field = string.Format("YEAR({0})", field);
                    break;

                case DateGroupingType.Month:
                    field = string.Format("CONVERT(VARCHAR(7),{0},120)", field);
                    break;

                case DateGroupingType.Day:
                    field = string.Format("CONVERT(VARCHAR(10),{0},120)", field);
                    break;

                case DateGroupingType.Quarter:
                    field = string.Format("DATEPART(QUARTER,{0})", field);
                    break;

                case DateGroupingType.Week:
                    field = string.Format("DATEPART(WEEK,{0})", field);
                    break;
            }
            return field;
        }

        public static string GetAggregationExpression(AggregateType at, string field)
        {
            return string.Format("{0}({1})", Enum.GetName(typeof(AggregateType), at), field);
        }
    }
}