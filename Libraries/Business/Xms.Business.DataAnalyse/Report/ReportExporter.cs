using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Business.DataAnalyse.Report
{
    /// <summary>
    /// 报表导出XML
    /// </summary>
    public class ReportExporter : ISolutionComponentExporter
    {
        private readonly IReportService _reportService;

        public ReportExporter(IReportService reportService)
        {
            _reportService = reportService;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _reportService.QueryPaged(x => x.Page(page, pageSize), solutionId, true);
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