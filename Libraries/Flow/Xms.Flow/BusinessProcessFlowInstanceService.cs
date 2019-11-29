using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Flow.Data;
using Xms.Flow.Domain;

namespace Xms.Flow
{
    /// <summary>
    /// 业务流程服务
    /// </summary>
    public class BusinessProcessFlowInstanceService : IBusinessProcessFlowInstanceService
    {
        private readonly IBusinessProcessFlowInstanceRepository _businessProcessFlowInstanceRepository;

        public BusinessProcessFlowInstanceService(IBusinessProcessFlowInstanceRepository businessProcessFlowInstanceRepository)
        {
            _businessProcessFlowInstanceRepository = businessProcessFlowInstanceRepository;
        }

        public bool Create(BusinessProcessFlowInstance entity)
        {
            return _businessProcessFlowInstanceRepository.Create(entity);
        }

        public bool CreateMany(List<BusinessProcessFlowInstance> entities)
        {
            return _businessProcessFlowInstanceRepository.CreateMany(entities);
        }

        public BusinessProcessFlowInstance FindById(Guid id)
        {
            return _businessProcessFlowInstanceRepository.FindById(id);
        }

        public BusinessProcessFlowInstance Find(Expression<Func<BusinessProcessFlowInstance, bool>> predicate)
        {
            return _businessProcessFlowInstanceRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            return _businessProcessFlowInstanceRepository.DeleteById(id);
        }

        public bool DeleteById(List<Guid> ids)
        {
            return _businessProcessFlowInstanceRepository.DeleteMany(ids);
        }

        public bool DeleteByParentId(Guid parentid)
        {
            return _businessProcessFlowInstanceRepository.DeleteMany(x => x.WorkFlowId == parentid);
        }

        public bool DeleteByEntityId(Guid entityId)
        {
            return _businessProcessFlowInstanceRepository.DeleteMany(x => x.ProcessEntityId == entityId);
        }

        public PagedList<BusinessProcessFlowInstance> QueryPaged(Func<QueryDescriptor<BusinessProcessFlowInstance>, QueryDescriptor<BusinessProcessFlowInstance>> container)
        {
            QueryDescriptor<BusinessProcessFlowInstance> q = container(QueryDescriptorBuilder.Build<BusinessProcessFlowInstance>());

            return _businessProcessFlowInstanceRepository.QueryPaged(q);
        }

        public List<BusinessProcessFlowInstance> Query(Func<QueryDescriptor<BusinessProcessFlowInstance>, QueryDescriptor<BusinessProcessFlowInstance>> container)
        {
            QueryDescriptor<BusinessProcessFlowInstance> q = container(QueryDescriptorBuilder.Build<BusinessProcessFlowInstance>());

            return _businessProcessFlowInstanceRepository.Query(q)?.ToList();
        }
    }
}