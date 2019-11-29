using System.Collections.Generic;

namespace Xms.Business.DataAnalyse
{
    public class ChartContext
    {
        public ChartDescriptor Chart { get; set; }
        public ChartDataDescriptor ChartData { get; set; }
        public List<Schema.Domain.Attribute> Attributes { get; set; }

        public List<dynamic> DataSource { get; set; }
    }
}