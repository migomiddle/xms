using System;
using System.Collections.Generic;
using Xms.Core.Context;
using Xms.Security.Domain;

namespace Xms.Security.Resource
{
    public interface IResourceOwnerService
    {
        bool Create(ResourceOwner entity);

        bool DeleteById(Guid id);

        bool DeleteById(IEnumerable<Guid> ids);

        List<ResourceOwner> FindAll();

        ResourceOwner FindById(Guid id);

        ResourceOwner FindByName(string name);

        List<ResourceOwner> Query(Func<QueryDescriptor<ResourceOwner>, QueryDescriptor<ResourceOwner>> container);

        PagedList<ResourceOwner> QueryPaged(Func<QueryDescriptor<ResourceOwner>, QueryDescriptor<ResourceOwner>> container);

        bool Update(Func<UpdateContext<ResourceOwner>, UpdateContext<ResourceOwner>> context);

        bool Update(ResourceOwner entity);
    }
}