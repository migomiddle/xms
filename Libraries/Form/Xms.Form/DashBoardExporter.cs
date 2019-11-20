using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Form
{
    /// <summary>
    /// 仪表板导出XML
    /// </summary>
    public class DashBoardExporter : ISolutionComponentExporter
    {
        private readonly ISystemFormFinder _systemFormFinder;

        public DashBoardExporter(ISystemFormFinder systemFormFinder)
        {
            _systemFormFinder = systemFormFinder;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _systemFormFinder.QueryPaged(x => x.Where(f => f.FormType == (int)Abstractions.FormType.Dashboard).Page(page, pageSize), solutionId, true, Abstractions.FormType.Dashboard);
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