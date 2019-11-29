using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Flow.Data;
using Xms.Flow.Domain;
using Xms.Infrastructure.Utility;

namespace Xms.Flow
{
    /// <summary>
    /// 流程执行日志服务
    /// </summary>
    public class WorkFlowProcessLogService : IWorkFlowProcessLogService, ICascadeDelete<WorkFlowProcess>
    {
        private readonly IWorkFlowProcessLogRepository _workFlowProcessLogRepository;

        public WorkFlowProcessLogService(IWorkFlowProcessLogRepository workFlowProcessLogRepository)
        {
            _workFlowProcessLogRepository = workFlowProcessLogRepository;
        }

        public bool Create(WorkFlowProcessLog entity)
        {
            return _workFlowProcessLogRepository.Create(entity);
        }

        public bool CreateMany(IEnumerable<WorkFlowProcessLog> entities)
        {
            return _workFlowProcessLogRepository.CreateMany(entities);
        }

        public bool Update(WorkFlowProcessLog entity)
        {
            return _workFlowProcessLogRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<WorkFlowProcessLog>, UpdateContext<WorkFlowProcessLog>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<WorkFlowProcessLog>());
            return _workFlowProcessLogRepository.Update(ctx);
        }

        public WorkFlowProcessLog FindById(Guid id)
        {
            return _workFlowProcessLogRepository.FindById(id);
        }

        public WorkFlowProcessLog Find(Expression<Func<WorkFlowProcessLog, bool>> predicate)
        {
            return _workFlowProcessLogRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            return _workFlowProcessLogRepository.DeleteById(id);
        }

        public bool DeleteById(IEnumerable<Guid> ids)
        {
            return _workFlowProcessLogRepository.DeleteMany(ids);
        }

        public bool DeleteByParentId(Guid parentid)
        {
            return _workFlowProcessLogRepository.DeleteMany(x => x.WorkFlowInstanceId == parentid);
        }

        public PagedList<WorkFlowProcessLog> QueryPaged(Func<QueryDescriptor<WorkFlowProcessLog>, QueryDescriptor<WorkFlowProcessLog>> container)
        {
            QueryDescriptor<WorkFlowProcessLog> q = container(QueryDescriptorBuilder.Build<WorkFlowProcessLog>());

            return _workFlowProcessLogRepository.QueryPaged(q);
        }

        public List<WorkFlowProcessLog> Query(Func<QueryDescriptor<WorkFlowProcessLog>, QueryDescriptor<WorkFlowProcessLog>> container)
        {
            QueryDescriptor<WorkFlowProcessLog> q = container(QueryDescriptorBuilder.Build<WorkFlowProcessLog>());

            return _workFlowProcessLogRepository.Query(q)?.ToList();
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的审批流实例步骤</param>
        public void CascadeDelete(params WorkFlowProcess[] parent)
        {
            if (parent.NotEmpty())
            {
                _workFlowProcessLogRepository.DeleteMany(x => x.WorkFlowInstanceId.In(parent.Select(f => f.WorkFlowInstanceId)));
            }
        }
    }
}