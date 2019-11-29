using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Business.Filter
{
    /// <summary>
    /// 拦截规则导出XML
    /// </summary>
    public class FilterRuleExporter : ISolutionComponentExporter
    {
        private readonly IFilterRuleFinder _filterRuleFinder;

        public FilterRuleExporter(IFilterRuleFinder filterRuleFinder)
        {
            _filterRuleFinder = filterRuleFinder;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _filterRuleFinder.QueryPaged(x => x.Page(page, pageSize), solutionId, true);
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