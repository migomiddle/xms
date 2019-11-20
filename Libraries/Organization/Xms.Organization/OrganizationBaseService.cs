using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Organization.Data;
using Xms.Organization.Domain;

namespace Xms.Organization
{
    /// <summary>
    /// 组织基础信息服务
    /// </summary>
    public class OrganizationBaseService : IOrganizationBaseService
    {
        private readonly IOrganizationBaseRepository _repository;

        public OrganizationBaseService(IOrganizationBaseRepository organizationBaseRepository)
        {
            _repository = organizationBaseRepository;
        }

        public bool Create(OrganizationBase entity)
        {
            var flag = _repository.Create(entity);
            return flag;
        }

        public bool Update(OrganizationBase entity)
        {
            var flag = _repository.Update(entity);
            return flag;
        }

        //public bool Update(Func<UpdateContext<OrganizationBase>, UpdateContext<OrganizationBase>> context)
        //{
        //    var ctx = context(new UpdateContext<OrganizationBase>());
        //    return _repository.Update(ctx);
        //}

        public OrganizationBase FindById(Guid id)
        {
            var result = _repository.FindById(id);
            return result;
        }

        public OrganizationBase FindByUniqueName(string uniqueName)
        {
            var result = _repository.Find(n => n.UniqueName == uniqueName);
            return result;
        }

        public OrganizationBase Find(Expression<Func<OrganizationBase, bool>> predicate)
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

        public PagedList<OrganizationBase> QueryPaged(Func<QueryDescriptor<OrganizationBase>, QueryDescriptor<OrganizationBase>> container)
        {
            QueryDescriptor<OrganizationBase> q = container(QueryDescriptorBuilder.Build<OrganizationBase>());

            return _repository.QueryPaged(q);
        }

        public List<OrganizationBase> Query(Func<QueryDescriptor<OrganizationBase>, QueryDescriptor<OrganizationBase>> container)
        {
            QueryDescriptor<OrganizationBase> q = container(QueryDescriptorBuilder.Build<OrganizationBase>());

            return _repository.Query(q);
        }
    }
}