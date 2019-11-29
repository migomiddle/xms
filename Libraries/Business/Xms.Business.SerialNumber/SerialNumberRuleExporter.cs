using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Business.SerialNumber
{
    /// <summary>
    /// 自动编号规则导出XML
    /// </summary>
    public class SerialNumberRuleExporter : ISolutionComponentExporter
    {
        private readonly ISerialNumberRuleFinder _serialNumberRuleFinder;

        public SerialNumberRuleExporter(ISerialNumberRuleFinder serialNumberRuleFinder)
        {
            _serialNumberRuleFinder = serialNumberRuleFinder;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _serialNumberRuleFinder.QueryPaged(x => x.Page(page, pageSize), solutionId, true);
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