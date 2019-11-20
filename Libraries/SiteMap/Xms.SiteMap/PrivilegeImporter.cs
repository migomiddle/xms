using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Infrastructure.Utility;
using Xms.SiteMap.Domain;
using Xms.Solution.Abstractions;

namespace Xms.SiteMap
{
    /// <summary>
    /// 菜单导入服务
    /// </summary>
    [SolutionImportNode("privileges")]
    public class PrivilegeImporter : ISolutionComponentImporter<Privilege>
    {
        private readonly IPrivilegeService _privilegeService;
        private readonly IAppContext _appContext;

        public PrivilegeImporter(IAppContext appContext
            , IPrivilegeService privilegeService)
        {
            _appContext = appContext;
            _privilegeService = privilegeService;
        }

        public bool Import(Guid solutionId, IList<Privilege> privileges)
        {
            if (privileges.NotEmpty())
            {
                foreach (var item in privileges)
                {
                    var entity = _privilegeService.FindById(item.PrivilegeId);
                    if (entity != null)
                    {
                        entity.BigIcon = item.BigIcon;
                        entity.ClassName = item.ClassName;
                        entity.DisplayName = item.DisplayName;
                        entity.DisplayOrder = item.DisplayOrder;
                        entity.IsVisibled = item.IsVisibled;
                        entity.Level = item.Level;
                        entity.MethodName = item.MethodName;
                        entity.OpenTarget = item.OpenTarget;
                        entity.ParentPrivilegeId = item.ParentPrivilegeId;
                        entity.SmallIcon = item.SmallIcon;
                        entity.SystemName = item.SystemName;
                        entity.Url = item.Url;
                        entity.AuthorizationEnabled = item.AuthorizationEnabled;
                        entity.Description = item.Description;
                        _privilegeService.Update(entity);
                    }
                    else
                    {
                        item.OrganizationId = _appContext.OrganizationId;
                        _privilegeService.Create(item);
                    }
                }
            }

            return true;
        }
    }
}