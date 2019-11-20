using System;
using Xms.Core.Data;
using Xms.SiteMap.Domain;

namespace Xms.SiteMap.Data
{
    public interface IPrivilegeRepository : IRepository<Privilege>
    {
        int MoveNode(Guid moveid, Guid targetid, Guid parentid, string position);

        string GetPrivilegesXml(Guid solutionId);
    }
}