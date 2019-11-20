using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Data.Import.Data;
using Xms.Data.Import.Domain;
using Xms.Data.Provider;

namespace Xms.Data.Import
{
    /// <summary>
    /// 导入数据服务
    /// </summary>
    public class ImportDataService : IImportDataService
    {
        private readonly IImportDataRepository _importDataRepository;

        public ImportDataService(IImportDataRepository importDataRepository)
        {
            _importDataRepository = importDataRepository;
        }

        public bool Create(ImportData entity)
        {
            bool flag = _importDataRepository.Create(entity);
            return flag;
        }

        public bool Update(ImportData entity)
        {
            var flag = _importDataRepository.Update(entity);
            return flag;
        }

        public ImportData FindById(Guid id)
        {
            var result = _importDataRepository.FindById(id);
            return result;
        }

        public ImportData Find(Expression<Func<ImportData, bool>> predicate)
        {
            return _importDataRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            bool flag = _importDataRepository.DeleteById(id);
            return flag;
        }

        public bool DeleteById(List<Guid> ids)
        {
            bool flag = true;
            foreach (var id in ids)
            {
                flag = this.DeleteById(id);
            }
            return flag;
        }

        public PagedList<ImportData> QueryPaged(Func<QueryDescriptor<ImportData>, QueryDescriptor<ImportData>> container)
        {
            QueryDescriptor<ImportData> q = container(QueryDescriptorBuilder.Build<ImportData>());

            return _importDataRepository.QueryPaged(q);
        }

        public List<ImportData> Query(Func<QueryDescriptor<ImportData>, QueryDescriptor<ImportData>> container)
        {
            QueryDescriptor<ImportData> q = container(QueryDescriptorBuilder.Build<ImportData>());

            return _importDataRepository.Query(q)?.ToList();
        }
    }
}