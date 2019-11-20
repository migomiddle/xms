using System;
using System.Collections.Generic;
using Xms.Core.Context;
using Xms.SiteMap.Domain;

namespace Xms.SiteMap
{
    public interface IPrivilegeTreeBuilder
    {
        string Build(Func<QueryDescriptor<Privilege>, QueryDescriptor<Privilege>> container, bool nameLower = true);

        List<dynamic> Build(List<Privilege> privilegeList, Guid parentId);

        List<Privilege> GetTreePath(string url);

        List<Privilege> GetTreePath(string areaName, string className, string methodName);
    }
}