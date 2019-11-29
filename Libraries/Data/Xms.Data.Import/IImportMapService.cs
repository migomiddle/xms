using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Data.Import.Domain;

namespace Xms.Data.Import
{
    public interface IImportMapService
    {
        bool Create(ImportMap entity);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        ImportMap Find(Expression<Func<ImportMap, bool>> predicate);

        ImportMap FindById(Guid id);

        List<ImportMap> Query(Func<QueryDescriptor<ImportMap>, QueryDescriptor<ImportMap>> container);

        PagedList<ImportMap> QueryPaged(Func<QueryDescriptor<ImportMap>, QueryDescriptor<ImportMap>> container);

        bool Update(ImportMap entity);
    }
}