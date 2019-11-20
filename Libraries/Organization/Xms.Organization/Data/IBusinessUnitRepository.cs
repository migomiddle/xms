using System;
using System.Collections.Generic;
using Xms.Core.Data;
using Xms.Organization.Domain;

namespace Xms.Organization.Data
{
    public interface IBusinessUnitRepository : IRepository<BusinessUnit>
    {
        List<BusinessUnit> GetChilds(Guid parentId);

        bool IsChild(Guid parentId, Guid businessUnitId);
    }
}