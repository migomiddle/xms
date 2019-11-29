using System;
using Xms.Context;
using Xms.Event.Abstractions;
using Xms.Flow.Abstractions;
using Xms.Flow.Core;
using Xms.Flow.Core.Events;
using Xms.Identity;
using Xms.Localization.Abstractions;
using Xms.Logging.AppLog;

namespace Xms.Flow
{
    /// <summary>
    /// 工作流撤消执行器
    /// </summary>
    public class WorkFlowCanceller : IWorkFlowCanceller
    {
        private readonly IWorkFlowInstanceService _WorkFlowInstanceService;
        private readonly IWorkFlowProcessUpdater _workFlowProcessUpdater;
        private readonly IWorkFlowProcessFinder _workFlowProcessFinder;
        private readonly ILocalizedTextProvider _loc;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAppContext _appContext;
        private readonly ICurrentUser _user;
        private readonly ILogService _logService;

        public WorkFlowCanceller(IAppContext appContext
            , IWorkFlowInstanceService WorkFlowInstanceService
            , IWorkFlowProcessUpdater workFlowProcessUpdater
            , IWorkFlowProcessFinder workFlowProcessFinder
            , IEventPublisher eventPublisher
            , ILogService logService)
        {
            _appContext = appContext;
            _loc = _appContext.GetFeature<ILocalizedTextProvider>();
            _user = _appContext.GetFeature<ICurrentUser>();
            _WorkFlowInstanceService = WorkFlowInstanceService;
            _workFlowProcessUpdater = workFlowProcessUpdater;
            _workFlowProcessFinder = workFlowProcessFinder;
            _eventPublisher = eventPublisher;
            _logService = logService;
        }

        /// <summary>
        /// 撤消工作流
        /// </summary>
        /// <param name="entityid"></param>
        /// <param name="objectid"></param>
        /// <returns></returns>
        public WorkFlowCancellationResult Cancel(WorkFlowCancellationContext context)
        {
            WorkFlowCancellationResult result = new WorkFlowCancellationResult();
            result.IsSuccess = true;
            var Instance = _WorkFlowInstanceService.Find(n => n.EntityId == context.EntityMetaData.EntityId && n.ObjectId == context.ObjectId && n.StateCode == WorkFlowProcessState.Processing);
            if (Instance == null)
            {
                result.IsSuccess = false;
                result.Message = _loc["workflow_noinstance"];
            }
            else if (Instance.ApplicantId != _user.SystemUserId)
            {
                result.IsSuccess = false;
                result.Message = _loc["workflow_nopermissioncancel"];
            }
            else if (Instance.StateCode != WorkFlowProcessState.Processing)
            {
                result.IsSuccess = false;
                result.Message = _loc["workflow_nopermissioncancel"];
            }
            var currentStep = _workFlowProcessFinder.Find(n => n.WorkFlowInstanceId == Instance.WorkFlowInstanceId && n.StateCode == WorkFlowProcessState.Processing);
            if (result.IsSuccess)
            {
                if (currentStep != null && !currentStep.AllowCancel)
                {
                    result.IsSuccess = false;
                    result.Message = _loc["workflow_notallowcancel"];
                }
            }
            result = OnCancelling(context, result);
            _eventPublisher.Publish(new WorkFlowCancellingEvent { ObjectId = context.ObjectId, EntityMetaData = context.EntityMetaData, Instance = Instance, CurrentStep = currentStep, Result = result });
            if (result.IsSuccess)
            {
                try
                {
                    _WorkFlowInstanceService.BeginTransaction();
                    _WorkFlowInstanceService.Update(n => n
                    .Set(f => f.StateCode, WorkFlowProcessState.Canceled)
                    .Set(f => f.CompletedOn, DateTime.Now)
                    .Where(f => f.WorkFlowInstanceId == Instance.WorkFlowInstanceId)
                    );
                    _workFlowProcessUpdater.Update(n => n.Set(f => f.StateCode, WorkFlowProcessState.Disabled).Where(f => f.WorkFlowInstanceId == Instance.WorkFlowInstanceId));
                    //_workFlowProcessUpdater.UpdateObjectProcessState(context.EntityMetaData, context.ObjectId, WorkFlowProcessState.Canceled);
                    _WorkFlowInstanceService.CompleteTransaction();
                    result.IsSuccess = true;
                }
                catch (Exception e)
                {
                    _WorkFlowInstanceService.RollBackTransaction();
                    result.IsSuccess = false;
                    result.Message = e.Message;
                    _logService.Error(e);
                }
            }
            result = OnCancelled(context, result);
            _eventPublisher.Publish(new WorkFlowCancelledEvent { ObjectId = context.ObjectId, EntityMetaData = context.EntityMetaData, Instance = Instance, CurrentStep = currentStep, Result = result });
            return result;
        }

        public virtual WorkFlowCancellationResult OnCancelling(WorkFlowCancellationContext context, WorkFlowCancellationResult result)
        {
            return result;
        }

        public virtual WorkFlowCancellationResult OnCancelled(WorkFlowCancellationContext context, WorkFlowCancellationResult result)
        {
            return result;
        }
    }
}