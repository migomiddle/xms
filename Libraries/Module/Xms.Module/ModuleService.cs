using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Module.Data;

namespace Xms.Module
{
    /// <summary>
    /// 模块服务
    /// </summary>
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository _repository;

        public ModuleService(IModuleRepository moduleRepository)
        {
            _repository = moduleRepository;
        }

        public bool Create(Domain.Module entity)
        {
            var flag = _repository.Create(entity);
            return flag;
        }

        public bool Update(Domain.Module entity)
        {
            var flag = _repository.Update(entity);
            return flag;
        }

        public Domain.Module FindById(Guid id)
        {
            var result = _repository.FindById(id);
            return result;
        }

        public Domain.Module Find(Expression<Func<Domain.Module, bool>> predicate)
        {
            return _repository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            var deleted = this.FindById(id);
            if (deleted == null)
            {
                return false;
            }
            var flag = _repository.DeleteById(id);
            return flag;
        }

        public bool DeleteById(List<Guid> ids)
        {
            var flag = true;
            foreach (var id in ids)
            {
                flag = this.DeleteById(id);
            }
            return flag;// _repository.DeleteById(ids);
        }

        public PagedList<Domain.Module> QueryPaged(Func<QueryDescriptor<Domain.Module>, QueryDescriptor<Domain.Module>> container)
        {
            QueryDescriptor<Domain.Module> q = container(QueryDescriptorBuilder.Build<Domain.Module>());

            return _repository.QueryPaged(q);
        }

        public List<Domain.Module> Query(Func<QueryDescriptor<Domain.Module>, QueryDescriptor<Domain.Module>> container)
        {
            QueryDescriptor<Domain.Module> q = container(QueryDescriptorBuilder.Build<Domain.Module>());

            return _repository.Query(q);
        }
    }
}