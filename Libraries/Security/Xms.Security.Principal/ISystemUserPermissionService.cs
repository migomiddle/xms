using System;
using System.Collections.Generic;
using Xms.SiteMap.Domain;

namespace Xms.Security.Principal
{
    public interface ISystemUserPermissionService
    {
        Privilege GetAuthPrivilege(Guid systemUserId, string areaName, string className, string methodName);

        Privilege GetAuthPrivilege(Guid systemUserId, string url);

        List<Guid> GetNoneReadFields(Guid systemUserId, List<Guid> attributes);

        List<Guid> GetNoneEditFields(Guid systemUserId, List<Guid> securityFields);

        List<Privilege> GetPrivileges(Guid systemUserId);
    }
}