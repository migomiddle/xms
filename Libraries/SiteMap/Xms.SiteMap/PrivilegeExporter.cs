using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.SiteMap
{
    /// <summary>
    /// 菜单导出XML
    /// </summary>
    public class PrivilegeExporter : ISolutionComponentExporter
    {
        private readonly IPrivilegeService _privilegeService;

        public PrivilegeExporter(IPrivilegeService privilegeService)
        {
            _privilegeService = privilegeService;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _privilegeService.QueryPaged(x => x.Page(page, pageSize));
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