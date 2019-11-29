using System;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Flow.Data;
using Xms.Flow.Domain;
using Xms.Sdk.Client;

namespace Xms.Flow
{
    /// <summary>
    /// 审批流状态更新服务
    /// </summary>
    public class WorkFlowProcessUpdater : IWorkFlowProcessUpdater
    {
        private readonly IWorkFlowProcessRepository _workFlowProcessRepository;
        private readonly IDataUpdater _dataUpdater;

        public WorkFlowProcessUpdater(IWorkFlowProcessRepository workFlowProcessRepository
            , IDataUpdater dataUpdater)
        {
            _workFlowProcessRepository = workFlowProcessRepository;
            _dataUpdater = dataUpdater;
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public bool Update(WorkFlowProcess entity)
        {
            return _workFlowProcessRepository.Update(entity);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public bool Update(Func<UpdateContext<WorkFlowProcess>, UpdateContext<WorkFlowProcess>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<WorkFlowProcess>());
            return _workFlowProcessRepository.Update(ctx);
        }
    }
}