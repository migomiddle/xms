using System;
using System.Collections.Generic;
using Xms.SiteMap.Domain;

namespace Xms.Security.Principal.Data
{
    public interface ISystemUserPermissionRepository
    {
        Privilege GetAuthPrivilege(Guid userId, string areaName, string className, string methodName, int objectTypeCode);

        Privilege GetAuthPrivilege(Guid userId, string url, int objectTypeCode);

        List<Guid> GetNoneReadFields(Guid userId, List<Guid> securityFields, int objectTypeCode);

        List<Guid> GetNoneEditFields(Guid userId, List<Guid> securityFields, int objectTypeCode);

        List<Privilege> GetPrivileges(Guid userId, int objectTypeCode);
    }
}