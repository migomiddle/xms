using System;
using System.Collections.Generic;
using Xms.Core.Context;
using Xms.SiteMap.Domain;

namespace Xms.SiteMap
{
    public interface IPrivilegeService
    {
        List<Privilege> AllPrivileges { get; }

        bool Create(Privilege entity);

        bool DeleteById(Guid id);

        bool DeleteById(IEnumerable<Guid> ids);

        Privilege Find(Func<QueryDescriptor<Privilege>, QueryDescriptor<Privilege>> container);

        Privilege Find(string systemName, string className, string methodName);

        Privilege Find(string url);

        Privilege FindById(Guid id);

        bool IsExists(Privilege entity);

        int Move(Guid moveid, Guid targetid, Guid parentid, string position);

        List<Privilege> Query(Func<QueryDescriptor<Privilege>, QueryDescriptor<Privilege>> container);

        PagedList<Privilege> QueryPaged(Func<QueryDescriptor<Privilege>, QueryDescriptor<Privilege>> container);

        bool Update(Privilege entity);

        bool UpdateAuthorization(bool isAuthorization, params Guid[] id);
    }
}