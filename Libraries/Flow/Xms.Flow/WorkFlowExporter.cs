using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Flow
{
    /// <summary>
    /// 审批流导出XML
    /// </summary>
    public class WorkFlowExporter : ISolutionComponentExporter
    {
        private readonly IWorkFlowFinder _workFlowFinder;

        public WorkFlowExporter(IWorkFlowFinder workFlowFinder)
        {
            _workFlowFinder = workFlowFinder;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _workFlowFinder.QueryPaged(x => x.Page(page, pageSize), solutionId, true);
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