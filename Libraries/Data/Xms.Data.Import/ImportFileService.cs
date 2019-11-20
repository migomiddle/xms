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
    /// 导入数据文件服务
    /// </summary>
    public class ImportFileService : IImportFileService
    {
        private readonly IImportFileRepository _importFileRepository;

        public ImportFileService(IImportFileRepository importFileRepository)
        {
            _importFileRepository = importFileRepository;
        }

        public bool Create(ImportFile entity)
        {
            bool flag = _importFileRepository.Create(entity);
            return flag;
        }

        public bool Update(ImportFile entity)
        {
            var flag = _importFileRepository.Update(entity);
            return flag;
        }

        public ImportFile FindById(Guid id)
        {
            var result = _importFileRepository.FindById(id);
            return result;
        }

        public ImportFile Find(Expression<Func<ImportFile, bool>> predicate)
        {
            return _importFileRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            bool flag = _importFileRepository.DeleteById(id);
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

        public PagedList<ImportFile> QueryPaged(Func<QueryDescriptor<ImportFile>, QueryDescriptor<ImportFile>> container)
        {
            QueryDescriptor<ImportFile> q = container(QueryDescriptorBuilder.Build<ImportFile>());

            return _importFileRepository.QueryPaged(q);
        }

        public List<ImportFile> Query(Func<QueryDescriptor<ImportFile>, QueryDescriptor<ImportFile>> container)
        {
            QueryDescriptor<ImportFile> q = container(QueryDescriptorBuilder.Build<ImportFile>());

            return _importFileRepository.Query(q)?.ToList();
        }
    }
}