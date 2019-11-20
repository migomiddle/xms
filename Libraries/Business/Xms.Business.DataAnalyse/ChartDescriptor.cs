using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Xms.Business.DataAnalyse
{
    public sealed class ChartDescriptor
    {
        public string ItemColor { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }

        public List<string> Legend { get; set; }

        public XAxis XAxis { get; set; }

        public YAxis YAxis { get; set; }

        public List<Series> Series { get; set; }
    }

    public sealed class XAxis
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public AxisType Type { get; set; }

        public List<string> Data { get; set; }
    }

    public sealed class YAxis
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public AxisType Type { get; set; }

        public string Formatter { get; set; }
    }

    public sealed class Series
    {
        public string Name { get; set; }
        public string ItemColor { get; set; }

        /// <summary>
        /// bar, line, funnel
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SeriesType Type { get; set; }

        public List<string> Data { get; set; }
    }

    public enum SeriesType
    {
        Bar,
        HBar,
        Line,
        Pie,
        Funnel,
        Gauge
    }

    public enum AxisType
    {
        Category,
        Value
    }
}