using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Data.Import.Domain;

namespace Xms.Data.Import
{
    public interface IImportFileService
    {
        bool Create(ImportFile entity);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        ImportFile Find(Expression<Func<ImportFile, bool>> predicate);

        ImportFile FindById(Guid id);

        List<ImportFile> Query(Func<QueryDescriptor<ImportFile>, QueryDescriptor<ImportFile>> container);

        PagedList<ImportFile> QueryPaged(Func<QueryDescriptor<ImportFile>, QueryDescriptor<ImportFile>> container);

        bool Update(ImportFile entity);
    }
}