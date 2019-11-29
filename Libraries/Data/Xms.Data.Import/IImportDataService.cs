using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Data.Import.Domain;

namespace Xms.Data.Import
{
    public interface IImportDataService
    {
        bool Create(ImportData entity);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        ImportData Find(Expression<Func<ImportData, bool>> predicate);

        ImportData FindById(Guid id);

        List<ImportData> Query(Func<QueryDescriptor<ImportData>, QueryDescriptor<ImportData>> container);

        PagedList<ImportData> QueryPaged(Func<QueryDescriptor<ImportData>, QueryDescriptor<ImportData>> container);

        bool Update(ImportData entity);
    }
}