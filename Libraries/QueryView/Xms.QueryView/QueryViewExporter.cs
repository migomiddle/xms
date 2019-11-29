using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.QueryView
{
    /// <summary>
    /// 视图导出XML
    /// </summary>
    public class QueryViewExporter : ISolutionComponentExporter
    {
        private readonly IQueryViewFinder _queryViewFinder;

        public QueryViewExporter(IQueryViewFinder queryViewFinder)
        {
            _queryViewFinder = queryViewFinder;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _queryViewFinder.QueryPaged(x => x.Page(page, pageSize), solutionId, true);
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