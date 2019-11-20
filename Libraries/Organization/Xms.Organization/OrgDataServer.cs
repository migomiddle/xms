using Microsoft.AspNetCore.Http;
using System;
using Xms.Core.Org;
using Xms.Infrastructure.Utility;

namespace Xms.Organization
{
    /// <summary>
    /// 组织数据服务器信息
    /// </summary>
    public class OrgDataServer : IOrgDataServer
    {
        public OrgDataServer(IHttpContextAccessor httpContext, IOrganizationBaseService organizationBaseService)
        {
            var uniqueName = httpContext.HttpContext.GetRouteOrQueryString("org")?.ToString();
            if (uniqueName.IsNotEmpty())
            {
                var baseOrg = organizationBaseService.FindByUniqueName(uniqueName);
                if (baseOrg != null)
                {
                    this.OrganizationBaseId = baseOrg.OrganizationBaseId;
                    this.DataServerName = baseOrg.DataServerName;
                    this.DataAccountName = baseOrg.DataAccountName;
                    this.DataPassword = baseOrg.DataPassword;
                    this.DatabaseName = baseOrg.DatabaseName;
                    this.UniqueName = baseOrg.UniqueName;
                }
            }
        }

        public Guid OrganizationBaseId { get; set; }
        public string DataServerName { get; set; }
        public string DataAccountName { get; set; }
        public string DataPassword { get; set; }
        public string DatabaseName { get; set; }
        public string UniqueName { get; set; }
    }
}