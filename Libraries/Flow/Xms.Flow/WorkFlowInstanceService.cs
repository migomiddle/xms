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
    /// 工作流实例服务
    /// </summary>
    public class WorkFlowInstanceService : IWorkFlowInstanceService
    {
        private readonly IWorkFlowInstanceRepository _workFlowInstanceRepository;

        public WorkFlowInstanceService(IWorkFlowInstanceRepository WorkFlowInstanceRepository)
        {
            _workFlowInstanceRepository = WorkFlowInstanceRepository;
        }

        public void BeginTransaction()
        {
            _workFlowInstanceRepository.DbContext.BeginTransaction();
        }

        public void CompleteTransaction()
        {
            _workFlowInstanceRepository.DbContext.CompleteTransaction();
        }

        public void RollBackTransaction()
        {
            _workFlowInstanceRepository.DbContext.RollBackTransaction();
        }

        public bool Create(WorkFlowInstance entity)
        {
            return _workFlowInstanceRepository.Create(entity);
        }

        public bool CreateMany(List<WorkFlowInstance> entities)
        {
            return _workFlowInstanceRepository.CreateMany(entities);
        }

        public bool Update(WorkFlowInstance entity)
        {
            return _workFlowInstanceRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<WorkFlowInstance>, UpdateContext<WorkFlowInstance>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<WorkFlowInstance>());
            return _workFlowInstanceRepository.Update(ctx);
        }

        public WorkFlowInstance FindById(Guid id)
        {
            return _workFlowInstanceRepository.FindById(id);
        }

        public WorkFlowInstance Find(Expression<Func<WorkFlowInstance, bool>> predicate)
        {
            return _workFlowInstanceRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            return _workFlowInstanceRepository.DeleteById(id);
        }

        public bool DeleteById(List<Guid> ids)
        {
            return _workFlowInstanceRepository.DeleteMany(ids);
        }

        public bool DeleteByParentId(Guid parentid)
        {
            return _workFlowInstanceRepository.DeleteMany(x => x.WorkFlowId == parentid);
        }

        public bool DeleteByObjectId(Guid entityId, Guid objectId)
        {
            return _workFlowInstanceRepository.DeleteMany(x => x.EntityId == entityId && x.ObjectId == objectId);
        }

        public List<WorkFlowInstance> Top(Func<QueryDescriptor<WorkFlowInstance>, QueryDescriptor<WorkFlowInstance>> container)
        {
            QueryDescriptor<WorkFlowInstance> q = container(QueryDescriptorBuilder.Build<WorkFlowInstance>());

            return _workFlowInstanceRepository.Top(q);
        }

        public PagedList<WorkFlowInstance> QueryPaged(Func<QueryDescriptor<WorkFlowInstance>, QueryDescriptor<WorkFlowInstance>> container)
        {
            QueryDescriptor<WorkFlowInstance> q = container(QueryDescriptorBuilder.Build<WorkFlowInstance>());

            return _workFlowInstanceRepository.QueryPaged(q);
        }

        public List<WorkFlowInstance> Query(Func<QueryDescriptor<WorkFlowInstance>, QueryDescriptor<WorkFlowInstance>> container)
        {
            QueryDescriptor<WorkFlowInstance> q = container(QueryDescriptorBuilder.Build<WorkFlowInstance>());

            return _workFlowInstanceRepository.Query(q)?.ToList();
        }
    }
}