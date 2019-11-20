using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Organization.Domain;

namespace Xms.Organization.Data
{
    public interface IOrganizationBaseRepository
    {
        bool Create(OrganizationBase entity);

        bool CreateMany(List<OrganizationBase> entities);

        bool Update(OrganizationBase entity);

        bool Update(UpdateContext<OrganizationBase> context);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        long Count(QueryDescriptor<OrganizationBase> q);

        PagedList<OrganizationBase> QueryPaged(QueryDescriptor<OrganizationBase> q);

        List<OrganizationBase> Query(QueryDescriptor<OrganizationBase> q);

        OrganizationBase FindById(Guid id);

        OrganizationBase Find(Expression<Func<OrganizationBase, bool>> predicate);
    }
}