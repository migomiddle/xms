using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.WebResource
{
    /// <summary>
    /// Web资源导出XML
    /// </summary>
    public class WebResourceExporter : ISolutionComponentExporter
    {
        private readonly IWebResourceFinder _webResourceFinder;

        public WebResourceExporter(IWebResourceFinder webResourceFinder)
        {
            _webResourceFinder = webResourceFinder;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _webResourceFinder.QueryPaged(x => x.Page(page, pageSize), solutionId, true);
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