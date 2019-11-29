using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Plugin
{
    /// <summary>
    /// 实体插件导出XML
    /// </summary>
    public class EntityPluginExporter : ISolutionComponentExporter
    {
        private readonly IEntityPluginFinder _entityPluginFinder;

        public EntityPluginExporter(IEntityPluginFinder entityPluginFinder)
        {
            _entityPluginFinder = entityPluginFinder;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _entityPluginFinder.QueryPaged(x => x.Page(page, pageSize), solutionId, true);
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