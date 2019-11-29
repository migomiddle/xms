using System.Collections.Generic;
using Xms.Configuration.Domain;
using Xms.Context;
using Xms.Core.Org;
using Xms.Identity;
using Xms.Localization.Abstractions;
using Xms.SiteMap.Domain;

namespace Xms.Web.Framework.Context
{
    public interface IWebAppContext : IAppContext
    {
        /// <summary>
        /// 组织信息
        /// </summary>
        Organization.Domain.Organization Org { get; set; }

        /// <summary>
        /// 平台参数
        /// </summary>
        PlatformSetting PlatformSettings { get; set; }

        /// <summary>
        /// 本地化标签服务
        /// </summary>
        ILocalizedTextProvider T { get; set; }

        /// <summary>
        /// 当前用户信息
        /// </summary>
        ICurrentUser CurrentUser { get; set; }

        /// <summary>
        /// 组织数据存储参数
        /// </summary>
        IOrgDataServer OrgDataServer { get; }

        /// <summary>
        /// 菜单树结构数据
        /// </summary>
        List<Privilege> PrivilegeTree { get; }
    }
}