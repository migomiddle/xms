using System;
using System.Collections.Generic;
using Xms.Security.Domain;

namespace Xms.Sdk.Client
{
    public interface IDataSharer
    {
        bool Share(Guid entityId, Guid recordId, List<PrincipalObjectAccess> principals, bool ignorePermissions = false);
    }
}