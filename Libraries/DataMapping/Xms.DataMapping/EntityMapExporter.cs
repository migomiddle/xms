using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.DataMapping
{
    /// <summary>
    /// 数据映射导出XML
    /// </summary>
    public class EntityMapExporter : ISolutionComponentExporter
    {
        private readonly IEntityMapFinder _entityMapFinder;

        public EntityMapExporter(IEntityMapFinder entityMapFinder)
        {
            _entityMapFinder = entityMapFinder;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _entityMapFinder.QueryPaged(x => x.Page(page, pageSize), solutionId, true);
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