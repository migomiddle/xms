using System;
using System.Linq;
using System.Threading.Tasks;
using Xms.Event.Abstractions;
using Xms.File;
using Xms.Flow.Abstractions;
using Xms.Flow.Core;
using Xms.Flow.Core.Events;
using Xms.Infrastructure.Utility;
using Xms.Logging.AppLog;

namespace Xms.Flow
{
    /// <summary>
    /// 工作流处理执行器
    /// </summary>
    public class WorkFlowExecuter : IWorkFlowExecuter
    {
        private readonly IWorkFlowInstanceService _workFlowInstanceService;
        private readonly IWorkFlowProcessUpdater _workFlowProcessUpdater;
        private readonly IWorkFlowProcessFinder _workFlowProcessFinder;
        private readonly IWorkFlowHandlerFinder _workFlowHandlerFinder;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAttachmentCreater _attachmentCreater;
        private readonly ILogService _logService;

        public WorkFlowExecuter(IWorkFlowInstanceService WorkFlowInstanceService
            , IWorkFlowProcessUpdater workFlowProcessUpdater
            , IWorkFlowProcessFinder workFlowProcessFinder
            , IWorkFlowHandlerFinder workFlowHandlerFinder
            , IEventPublisher eventPublisher
            , IAttachmentCreater attachmentCreater
            , ILogService logService)
        {
            _workFlowInstanceService = WorkFlowInstanceService;
            _workFlowProcessUpdater = workFlowProcessUpdater;
            _workFlowProcessFinder = workFlowProcessFinder;
            _workFlowHandlerFinder = workFlowHandlerFinder;
            _eventPublisher = eventPublisher;
            _attachmentCreater = attachmentCreater;
            _logService = logService;
        }

        /// <summary>
        /// 执行工作流
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<WorkFlowExecutionResult> ExecuteAsync(WorkFlowExecutionContext context)
        {
            var result = new WorkFlowExecutionResult();
            OnExecuting(context, result);
            _eventPublisher.Publish(new WorkFlowExecutingEvent { Context = context });
            try
            {
                _workFlowInstanceService.BeginTransaction();
                //更新当前步骤处理状态
                _workFlowProcessUpdater.Update(n => n
                    .Set(f => f.StateCode, context.ProcessState)
                    .Set(f => f.HandleTime, DateTime.Now)
                    .Set(f => f.Description, context.Description)
                    .Set(f => f.Attachments, context.Attachments)
                    .Where(f => f.WorkFlowProcessId == context.ProcessInfo.WorkFlowProcessId)
                );
                //上传附件
                await _attachmentCreater.CreateManyAsync(context.EntityMetaData.EntityId, context.ProcessInfo.WorkFlowProcessId, context.AttachmentFiles).ConfigureAwait(false);
                //更新当前步骤其他处理者状态
                _workFlowProcessUpdater.Update(n => n
                    .Set(f => f.StateCode, WorkFlowProcessState.Disabled)
                    .Where(f => f.WorkFlowInstanceId == context.ProcessInfo.WorkFlowInstanceId && f.StepOrder == context.ProcessInfo.StepOrder && f.WorkFlowProcessId != context.ProcessInfo.WorkFlowProcessId)
                );
                var nextI = context.ProcessInfo.StepOrder + 1;
                var nextSteps = _workFlowProcessFinder.Query(n => n.Where(f => f.WorkFlowInstanceId == context.ProcessInfo.WorkFlowInstanceId && f.StepOrder == nextI));
                //如果到了最后一个环节
                if (nextSteps.IsEmpty())
                {
                    //如果同意
                    if (context.ProcessState == WorkFlowProcessState.Passed)
                    {
                        //更新本次申请状态为完成
                        _workFlowInstanceService.Update(n => n
                            .Set(f => f.StateCode, context.ProcessState)
                                .Set(f => f.CompletedOn, DateTime.Now)
                            .Where(f => f.WorkFlowInstanceId == context.InstanceInfo.WorkFlowInstanceId)
                        );
                    }
                    //如果驳回
                    else if (context.ProcessState == WorkFlowProcessState.UnPassed)
                    {
                        //更新当前流程实例状态为完成
                        _workFlowInstanceService.Update(n => n
                            .Set(f => f.StateCode, context.ProcessState)
                                .Set(f => f.CompletedOn, DateTime.Now)
                            .Where(f => f.WorkFlowInstanceId == context.InstanceInfo.WorkFlowInstanceId)
                        );
                    }
                }
                //如果还有下一步
                else
                {
                    //驳回
                    if (context.ProcessState == WorkFlowProcessState.UnPassed)
                    {
                        //更新当前流程实例状态为完成
                        _workFlowInstanceService.Update(n => n
                            .Set(f => f.StateCode, context.ProcessState)
                            .Set(f => f.CompletedOn, DateTime.Now)
                            .Where(f => f.WorkFlowInstanceId == context.InstanceInfo.WorkFlowInstanceId)
                        );
                        //更新未处理的步骤状态为作废
                        _workFlowProcessUpdater.Update(n => n.Set(f => f.StateCode, WorkFlowProcessState.Disabled).Where(f => f.WorkFlowInstanceId == context.InstanceInfo.WorkFlowInstanceId && f.StateCode == WorkFlowProcessState.Waiting));
                    }
                    //同意则转交到下一个处理人
                    else if (context.ProcessState == WorkFlowProcessState.Passed)
                    {
                        //更新下一步骤处理状态为处理中
                        _workFlowProcessUpdater.Update(n => n.Set(f => f.StateCode, WorkFlowProcessState.Processing).Set(f => f.StartTime, DateTime.Now)
                        .Where(f => f.WorkFlowInstanceId == context.InstanceInfo.WorkFlowInstanceId && f.StepOrder == nextI));

                        result.NextHandlerId = _workFlowHandlerFinder.GetCurrentHandlerId(context.InstanceInfo, context.ProcessInfo, nextSteps.First().HandlerIdType, nextSteps.First().Handlers);
                    }
                }
                _workFlowInstanceService.CompleteTransaction();
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                _workFlowInstanceService.RollBackTransaction();
                result.IsSuccess = false;
                result.Message = e.Message;
                _logService.Error(e);
            }
            OnExecuted(context, result);
            //发布事件
            _eventPublisher.Publish(new WorkFlowExecutedEvent { Context = context, Result = result });
            return result;
        }

        #region 事件

        public virtual WorkFlowExecutionResult OnExecuting(WorkFlowExecutionContext context, WorkFlowExecutionResult result)
        {
            return result;
        }

        public virtual WorkFlowExecutionResult OnExecuted(WorkFlowExecutionContext context, WorkFlowExecutionResult result)
        {
            return result;
        }

        #endregion 事件
    }
}