using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Business.DuplicateValidator
{
    /// <summary>
    /// 重复规则导出XML
    /// </summary>
    public class DuplicateRuleExporter : ISolutionComponentExporter
    {
        private readonly IDuplicateRuleFinder _duplicateRuleFinder;

        public DuplicateRuleExporter(IDuplicateRuleFinder duplicateRuleFinder)
        {
            _duplicateRuleFinder = duplicateRuleFinder;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _duplicateRuleFinder.QueryPaged(x => x.Page(page, pageSize), solutionId, true);
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