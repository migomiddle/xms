using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Organization.Domain;

namespace Xms.Organization
{
    public interface ISystemUserService
    {
        bool Create(SystemUser entity);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        bool ExistsLoginName(string loginName, Guid? currentUserId);

        SystemUser Find(Expression<Func<SystemUser, bool>> predicate);

        SystemUser FindById(Guid id);

        SystemUser GetUserByLoginName(string loginName);

        SystemUser GetUserByLoginNameAndPassword(string loginName, string password, string salt = "");

        bool IsValidePassword(string inputPassword, string salt, string password);

        List<SystemUser> Query(Func<QueryDescriptor<SystemUser>, QueryDescriptor<SystemUser>> container);

        PagedList<SystemUser> QueryPaged(Func<QueryDescriptor<SystemUser>, QueryDescriptor<SystemUser>> container);

        bool Update(Func<UpdateContext<SystemUser>, UpdateContext<SystemUser>> context);

        bool Update(SystemUser entity);

        bool UpdateLastLoginTime(Guid userId);

        bool UpdateLastVisitTime(Guid userId);
    }
}