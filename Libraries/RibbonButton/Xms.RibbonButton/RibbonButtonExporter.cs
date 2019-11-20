using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.RibbonButton
{
    /// <summary>
    /// 按钮导出XML
    /// </summary>
    public class RibbonButtonExporter : ISolutionComponentExporter
    {
        private readonly IRibbonButtonFinder _ribbonButtonFinder;

        public RibbonButtonExporter(IRibbonButtonFinder ribbonButtonFinder)
        {
            _ribbonButtonFinder = ribbonButtonFinder;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _ribbonButtonFinder.QueryPaged(x => x.Page(page, pageSize), solutionId, true);
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