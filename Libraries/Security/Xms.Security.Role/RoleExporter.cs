using System;
using System.Text;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Security.Role
{
    /// <summary>
    /// 安全角色导出XML
    /// </summary>
    public class RoleExporter : ISolutionComponentExporter
    {
        private readonly IRoleService _roleService;

        public RoleExporter(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public string GetXml(Guid solutionId)
        {
            StringBuilder result = new StringBuilder();
            var pageSize = 100;
            var page = 1;
            long totalItems = pageSize;
            while (totalItems == pageSize)
            {
                var data = _roleService.QueryPaged(x => x.Page(page, pageSize));
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