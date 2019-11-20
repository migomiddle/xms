using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Schema.Entity
{
    /// <summary>
    /// 实体导出XML
    /// </summary>
    public class EntityExporter : ISolutionComponentExporter
    {
        private readonly IEntityFinder _entityFinder;

        public EntityExporter(IEntityFinder entityFinder)
        {
            _entityFinder = entityFinder;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _entityFinder.QueryPaged(x => x.Page(page, pageSize), solutionId, true);
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