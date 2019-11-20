using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Security.Domain;

namespace Xms.Security.DataAuthorization
{
    public interface IEntityPermissionService
    {
        bool Create(EntityPermission entity);

        bool CreateMany(List<EntityPermission> entities);

        bool CreateDefaultPermissions(Schema.Domain.Entity entity);

        bool DeleteById(Guid id);

        bool DeleteById(IEnumerable<Guid> ids);

        EntityPermission Find(Expression<Func<EntityPermission, bool>> predicate);

        EntityPermission FindById(Guid id);

        List<EntityPermission> Query(Func<QueryDescriptor<EntityPermission>, QueryDescriptor<EntityPermission>> container);

        PagedList<EntityPermission> QueryPaged(Func<QueryDescriptor<EntityPermission>, QueryDescriptor<EntityPermission>> container);

        bool Update(Func<UpdateContext<EntityPermission>, UpdateContext<EntityPermission>> context);

        bool Update(EntityPermission entity);
    }
}