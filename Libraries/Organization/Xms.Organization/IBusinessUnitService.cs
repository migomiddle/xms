using System;
using System.Collections.Generic;
using Xms.Core.Context;
using Xms.Organization.Domain;

namespace Xms.Organization
{
    public interface IBusinessUnitService
    {
        bool Create(BusinessUnit entity);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        BusinessUnit FindById(Guid id);

        List<BusinessUnit> GetChilds(Guid parentId);

        bool IsChild(Guid parentId, Guid businessUnitId);

        List<BusinessUnit> Query(Func<QueryDescriptor<BusinessUnit>, QueryDescriptor<BusinessUnit>> container);

        PagedList<BusinessUnit> QueryPaged(Func<QueryDescriptor<BusinessUnit>, QueryDescriptor<BusinessUnit>> container);

        bool Update(BusinessUnit entity);

        bool Update(Func<UpdateContext<BusinessUnit>, UpdateContext<BusinessUnit>> context);
    }
}