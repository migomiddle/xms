using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Organization.Data;

namespace Xms.Organization
{
    /// <summary>
    /// 组织业务信息服务
    /// </summary>
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;

        public OrganizationService(IOrganizationRepository organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }

        public bool Create(Domain.Organization entity)
        {
            return _organizationRepository.Create(entity);
        }

        public bool Update(Domain.Organization entity)
        {
            return _organizationRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<Domain.Organization>, UpdateContext<Domain.Organization>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<Domain.Organization>());
            return _organizationRepository.Update(ctx);
        }

        public Domain.Organization FindById(Guid id)
        {
            return _organizationRepository.FindById(id);
        }

        public Domain.Organization Find(Expression<Func<Domain.Organization, bool>> predicate)
        {
            return _organizationRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            //检查引用

            return _organizationRepository.DeleteById(id);
        }

        public bool DeleteById(IEnumerable<Guid> ids)
        {
            var flag = true;
            foreach (var id in ids)
            {
                flag = this.DeleteById(id);
            }
            return flag;// _repository.DeleteById(ids);
        }

        public PagedList<Domain.Organization> QueryPaged(Func<QueryDescriptor<Domain.Organization>, QueryDescriptor<Domain.Organization>> container)
        {
            QueryDescriptor<Domain.Organization> q = container(QueryDescriptorBuilder.Build<Domain.Organization>());

            return _organizationRepository.QueryPaged(q);
        }

        public List<Domain.Organization> Query(Func<QueryDescriptor<Domain.Organization>, QueryDescriptor<Domain.Organization>> container)
        {
            QueryDescriptor<Domain.Organization> q = container(QueryDescriptorBuilder.Build<Domain.Organization>());

            return _organizationRepository.Query(q)?.ToList();
        }
    }
}