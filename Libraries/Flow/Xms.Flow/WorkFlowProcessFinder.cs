using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Flow.Abstractions;
using Xms.Flow.Data;
using Xms.Flow.Domain;

namespace Xms.Flow
{
    /// <summary>
    /// 审批流执行步骤查找服务
    /// </summary>
    public class WorkFlowProcessFinder : IWorkFlowProcessFinder
    {
        private readonly IWorkFlowProcessRepository _workFlowProcessRepository;

        public WorkFlowProcessFinder(IWorkFlowProcessRepository workFlowProcessRepository)
        {
            _workFlowProcessRepository = workFlowProcessRepository;
        }

        /// <summary>
        /// 查找一条记录
        /// </summary>
        /// <param name="id">记录主键值</param>
        /// <returns></returns>
        public WorkFlowProcess FindById(Guid id)
        {
            return _workFlowProcessRepository.FindById(id);
        }

        /// <summary>
        /// 查找一条记录
        /// </summary>
        /// <param name="predicate">过滤条件表达式</param>
        /// <returns></returns>
        public WorkFlowProcess Find(Expression<Func<WorkFlowProcess, bool>> predicate)
        {
            return _workFlowProcessRepository.Find(predicate);
        }

        /// <summary>
        /// 分页查询记录
        /// </summary>
        /// <param name="container">查询表达式</param>
        /// <returns></returns>
        public PagedList<WorkFlowProcess> QueryPaged(Func<QueryDescriptor<WorkFlowProcess>, QueryDescriptor<WorkFlowProcess>> container)
        {
            QueryDescriptor<WorkFlowProcess> q = container(QueryDescriptorBuilder.Build<WorkFlowProcess>());

            return _workFlowProcessRepository.QueryPaged(q);
        }

        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="container">查询表达式</param>
        /// <returns></returns>
        public List<WorkFlowProcess> Query(Func<QueryDescriptor<WorkFlowProcess>, QueryDescriptor<WorkFlowProcess>> container)
        {
            QueryDescriptor<WorkFlowProcess> q = container(QueryDescriptorBuilder.Build<WorkFlowProcess>());

            return _workFlowProcessRepository.Query(q)?.ToList();
        }

        /// <summary>
        /// 查询已处理的记录
        /// </summary>
        /// <param name="handlerId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="entityid"></param>
        /// <returns></returns>
        public PagedList<dynamic> QueryHandledList(Guid handlerId, int page, int pageSize, Guid? entityid)
        {
            return _workFlowProcessRepository.QueryHandledList(handlerId, page, pageSize, entityid);
        }

        /// <summary>
        /// 查询正在处理的记录
        /// </summary>
        /// <param name="handlerId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="entityid"></param>
        /// <returns></returns>
        public PagedList<dynamic> QueryHandlingList(Guid handlerId, int page, int pageSize, Guid? entityid)
        {
            return _workFlowProcessRepository.QueryHandlingList(handlerId, page, pageSize, entityid);
        }

        /// <summary>
        /// 查询我提交申请的已处理的记录
        /// </summary>
        /// <param name="applierId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="entityid"></param>
        /// <returns></returns>
        public PagedList<dynamic> QueryApplyHandledList(Guid applierId, int page, int pageSize, Guid? entityid)
        {
            return _workFlowProcessRepository.QueryApplyHandledList(applierId, page, pageSize, entityid);
        }

        /// <summary>
        /// 查询我提交申请的正在处理的记录
        /// </summary>
        /// <param name="applierId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="entityid"></param>
        /// <returns></returns>
        public PagedList<dynamic> QueryApplyHandlingList(Guid applierId, int page, int pageSize, Guid? entityid)
        {
            return _workFlowProcessRepository.QueryApplyHandlingList(applierId, page, pageSize, entityid);
        }

        public long QueryHandledCount(Guid handlerId, Guid? entityid)
        {
            return _workFlowProcessRepository.QueryHandledCount(handlerId, entityid);
        }

        public long QueryHandlingCount(Guid handlerId, Guid? entityid)
        {
            return _workFlowProcessRepository.QueryHandlingCount(handlerId, entityid);
        }

        public long QueryApplyHandledCount(Guid applierId, Guid? entityid)
        {
            return _workFlowProcessRepository.QueryApplyHandledCount(applierId, entityid);
        }

        public long QueryApplyHandlingCount(Guid applierId, Guid? entityid)
        {
            return _workFlowProcessRepository.QueryApplyHandlingCount(applierId, entityid);
        }

        /// <summary>
        /// 获取当前步骤
        /// </summary>
        /// <param name="WorkFlowInstanceId"></param>
        /// <param name="handlerId"></param>
        /// <returns></returns>
        public WorkFlowProcess GetCurrentStep(Guid WorkFlowInstanceId, Guid handlerId)
        {
            var step = this.Find(f => f.WorkFlowInstanceId == WorkFlowInstanceId && f.StateCode == WorkFlowProcessState.Processing && f.HandlerId == handlerId);
            return step;
        }

        /// <summary>
        /// 获取最后已处理的步骤
        /// </summary>
        /// <param name="WorkFlowInstanceId"></param>
        /// <param name="handlerId"></param>
        /// <returns></returns>
        public WorkFlowProcess GetLastHandledStep(Guid WorkFlowInstanceId, Guid handlerId)
        {
            var step = this.Find(f => f.WorkFlowInstanceId == WorkFlowInstanceId && (f.StateCode == WorkFlowProcessState.Passed || f.StateCode == WorkFlowProcessState.UnPassed) && f.HandlerId == handlerId);
            return step;
        }
    }
}