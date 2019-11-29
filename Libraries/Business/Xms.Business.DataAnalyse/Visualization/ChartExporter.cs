using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Business.DataAnalyse.Visualization
{
    /// <summary>
    /// 图表导出XML
    /// </summary>
    public class ChartExporter : ISolutionComponentExporter
    {
        private readonly IChartFinder _chartFinder;

        public ChartExporter(IChartFinder chartFinder)
        {
            _chartFinder = chartFinder;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _chartFinder.QueryPaged(x => x.Page(page, pageSize), solutionId, true);
                totalItems = data.TotalItems;
                if (totalItems > 0)
                {
                    result.Append(data.Items.SerializeToXml());
                }
                page++;
            }
            return result.ToString();
        }
    }
}