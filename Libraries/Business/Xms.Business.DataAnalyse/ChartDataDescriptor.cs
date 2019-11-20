using System.Collections.Generic;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Business.DataAnalyse
{
    public sealed class ChartDataDescriptor
    {
        ////前X项或后X项
        //public TopDirectionType? TopDirection { get; set; }

        ////条目
        //public int? TopCount { get; set; }

        public List<ChartDataItem> Fetch { get; set; }
    }

    public sealed class ChartDataItem
    {
        //{"fetch":[{"attribute":"accountid", "type":"series", "aggregate":"count"},{"attribute":"createdon", "type":"category", "dategrouping":"month","groupby":true}]}
        public string Attribute { get; set; }

        public ChartItemType Type { get; set; }

        public AggregateType Aggregate { get; set; }

        public DateGroupingType? DateGrouping { get; set; }

        public bool? GroupBy { get; set; }
        public bool IgnoreNull { get; set; }

        //前X项或后X项
        public TopDirectionType? TopDirection { get; set; }

        //条目
        public int? TopCount { get; set; }
    }

    public enum TopDirectionType
    {
        Desc,
        Asc
    }

    public enum ChartItemType
    {
        Series,
        Category
    }
}