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
    /// 导入数据实体映射服务
    /// </summary>
    public class ImportMapService : IImportMapService
    {
        private readonly IImportMapRepository _importMapRepository;

        public ImportMapService(IImportMapRepository importMapRepository)
        {
            _importMapRepository = importMapRepository;
        }

        public bool Create(ImportMap entity)
        {
            bool flag = _importMapRepository.Create(entity);
            return flag;
        }

        public bool Update(ImportMap entity)
        {
            var flag = _importMapRepository.Update(entity);
            return flag;
        }

        public ImportMap FindById(Guid id)
        {
            var result = _importMapRepository.FindById(id);
            return result;
        }

        public ImportMap Find(Expression<Func<ImportMap, bool>> predicate)
        {
            return _importMapRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            bool flag = _importMapRepository.DeleteById(id);
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

        public PagedList<ImportMap> QueryPaged(Func<QueryDescriptor<ImportMap>, QueryDescriptor<ImportMap>> container)
        {
            QueryDescriptor<ImportMap> q = container(QueryDescriptorBuilder.Build<ImportMap>());

            return _importMapRepository.QueryPaged(q);
        }

        public List<ImportMap> Query(Func<QueryDescriptor<ImportMap>, QueryDescriptor<ImportMap>> container)
        {
            QueryDescriptor<ImportMap> q = container(QueryDescriptorBuilder.Build<ImportMap>());

            return _importMapRepository.Query(q)?.ToList();
        }
    }
}