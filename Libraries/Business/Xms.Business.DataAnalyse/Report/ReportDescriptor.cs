using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Business.DataAnalyse.Report
{
    public sealed class ReportDescriptor
    {
        public string Name { get; set; }
        public CustomReportDescriptor CustomReport { get; set; }

        public List<QueryExpression> ReportFilter { get; set; }
    }

    public sealed class ReportEntity
    {
        public string ParamName { get; set; }

        public QueryExpression Query { get; set; }
    }

    public sealed class CustomReportDescriptor
    {
        public QueryExpression Query { get; set; }
        public List<GroupingDescriptor> Groupings { get; set; }

        public List<ColumnDescriptor> Columns { get; set; }

        public FilterDescriptor Filter { get; set; }

        public ChartDescriptor Chart { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public TableLayoutType TableLayout { get; set; }
    }

    public enum TableLayoutType
    {
        Normal,
        Drillthrough
    }

    public sealed class GroupingDescriptor
    {
        public string Field { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public OrderType Sort { get; set; }

        public int Width { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public DateGroupingType? DateGrouping { get; set; }
    }

    public sealed class FilterDescriptor
    {
        public string Field { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public FilterOperator Operator { get; set; }

        public int Value { get; set; }
    }

    public enum FilterOperator
    {
        TopN,
        BottomN
    }

    public sealed class ColumnDescriptor
    {
        public string Id { get; set; }

        public string Field { get; set; }

        public int Width { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public AggregateType? SummaryValue { get; set; }
    }

    public sealed class ChartDescriptor
    {
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public ChartType Type { get; set; }

        public bool Legend { get; set; }

        public bool DataLabels { get; set; }

        public List<ValueAxisDescriptor> ValueAxes { get; set; }
        public ColumnAxisDescriptor ColumnAxis { get; set; }
    }

    public enum ChartType
    {
        Column,
        Bar,
        Line,
        Pie,
        Funnel
    }

    public sealed class ValueAxisDescriptor
    {
        public string Field { get; set; }

        public string Name { get; set; }
        //public AggregateType SummaryValue { get; set; }
    }

    public sealed class ColumnAxisDescriptor
    {
        public string Field { get; set; }

        public string Name { get; set; }
    }
}