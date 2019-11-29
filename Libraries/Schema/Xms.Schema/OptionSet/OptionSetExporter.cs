using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Schema.OptionSet
{
    /// <summary>
    /// 选项集导出XML
    /// </summary>
    public class OptionSetExporter : ISolutionComponentExporter
    {
        private readonly IOptionSetFinder _optionSetFinder;

        public OptionSetExporter(IOptionSetFinder optionSetFinder)
        {
            _optionSetFinder = optionSetFinder;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _optionSetFinder.QueryPaged(x => x.Page(page, pageSize), solutionId, true);
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