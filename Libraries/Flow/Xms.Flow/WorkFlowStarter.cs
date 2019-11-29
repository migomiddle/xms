using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xms.Context;
using Xms.Event.Abstractions;
using Xms.File;
using Xms.Flow.Abstractions;
using Xms.Flow.Core;
using Xms.Flow.Core.Events;
using Xms.Flow.Domain;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Logging.AppLog;
using Xms.Schema.Attribute;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Extensions;

namespace Xms.Flow
{
    /// <summary>
    /// 工作流启动器
    /// </summary>
    public class WorkFlowStarter : IWorkFlowStarter
    {
        private readonly IWorkFlowInstanceService _WorkFlowInstanceService;
        private readonly IWorkFlowStepService _workFlowStepService;
        private readonly IWorkFlowProcessService _workFlowProcessService;
        private readonly IWorkFlowProcessUpdater _workFlowProcessUpdater;
        private readonly IWorkFlowHandlerFinder _workFlowHandlerFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAttachmentCreater _attachmentCreater;
        private readonly ILogService _logService;
        private readonly ILocalizedTextProvider _loc;

        public WorkFlowStarter(IAppContext appContext
            , IWorkFlowInstanceService WorkFlowInstanceService
            , IWorkFlowStepService workFlowStepService
            , IWorkFlowProcessService workFlowProcessService
            , IWorkFlowProcessUpdater workFlowProcessUpdater
            , IWorkFlowHandlerFinder workFlowHandlerFinder
            , IEventPublisher eventPublisher
            , IAttributeFinder attributeFinder
            , IAttachmentCreater attachmentCreater
            , ILogService logService)
        {
            _loc = appContext.GetFeature<ILocalizedTextProvider>();
            _WorkFlowInstanceService = WorkFlowInstanceService;
            _workFlowStepService = workFlowStepService;
            _workFlowProcessService = workFlowProcessService;
            _workFlowProcessUpdater = workFlowProcessUpdater;
            _workFlowHandlerFinder = workFlowHandlerFinder;
            _eventPublisher = eventPublisher;
            _attributeFinder = attributeFinder;
            _attachmentCreater = attachmentCreater;
            _logService = logService;
        }

        /// <summary>
        /// 启动工作流
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<WorkFlowExecutionResult> StartAsync(WorkFlowStartUpContext context)
        {
            var result = new WorkFlowExecutionResult();
            result.IsSuccess = true;
            if (_WorkFlowInstanceService.Find(n => n.EntityId == context.WorkFlowMetaData.EntityId && n.ObjectId == context.ObjectId && n.StateCode == WorkFlowProcessState.Processing) != null)
            {
                result.IsSuccess = false;
                result.Message = _loc["workflow_processing_notallowcancel"];
            }
            if (result.IsSuccess)
            {
                var workFlowSteps = _workFlowStepService.Query(n => n
                .Where(f => f.WorkFlowId == context.WorkFlowMetaData.WorkFlowId)
                .Sort(s => s.SortAscending(f => f.StepOrder)));
                //生成审批任务
                var InstanceInfo = new WorkFlowInstance()
                {
                    ApplicantId = context.ApplicantId
                    ,
                    CreatedOn = DateTime.Now
                    ,
                    Description = context.Description
                    ,
                    EntityId = context.WorkFlowMetaData.EntityId
                    ,
                    ObjectId = context.ObjectId
                    ,
                    WorkFlowId = context.WorkFlowMetaData.WorkFlowId
                    ,
                    StateCode = WorkFlowProcessState.Processing
                    ,
                    Attachments = context.Attachments
                    ,
                    WorkFlowInstanceId = Guid.NewGuid()
                };
                //生成审批唯一码
                var randomService = new Randoms();
                //生成审批处理步骤
                var wfpList = new List<WorkFlowProcess>();
                //开始节点
                var startNode = workFlowSteps.Find(n => n.NodeType == 0);
                bool hasNext = true;
                WorkFlowStep nextStep = startNode;
                WorkFlowProcess prevStep = null;
                int stepOrder = 1;
                while (hasNext)
                {
                    //生成当前节点处理记录
                    if (nextStep.NodeType > 1)
                    {
                        var handlerIds = _workFlowHandlerFinder.GetCurrentHandlerId(InstanceInfo, prevStep, nextStep.HandlerIdType, nextStep.Handlers);
                        if (handlerIds.IsEmpty())
                        {
                            result.IsSuccess = false;
                            result.Message = _loc["workflow_step_nonehandler"].FormatWith(nextStep.Name);
                            break;
                        }
                        foreach (var hid in handlerIds)
                        {
                            var wfp = new WorkFlowProcess()
                            {
                                Name = nextStep.Name
                                ,
                                StateCode = WorkFlowProcessState.Waiting
                                ,
                                StepOrder = stepOrder
                                ,
                                UniqueCode = randomService.CreateRandomValue(6, true)
                                ,
                                AuthAttributes = nextStep.AuthAttributes
                                ,
                                FormId = nextStep.FormId
                                ,
                                AllowAssign = nextStep.AllowAssign
                                ,
                                AllowCancel = nextStep.AllowCancel
                                ,
                                HandlerIdType = nextStep.HandlerIdType
                                ,
                                Handlers = nextStep.Handlers
                                ,
                                HandlerId = hid
                                ,
                                ReturnType = nextStep.ReturnType
                                ,
                                ReturnTo = nextStep.ReturnTo
                                ,
                                AttachmentRequired = nextStep.AttachmentRequired
                                ,
                                AttachmentExts = nextStep.AttachmentExts
                                ,
                                //Conditions = step.Conditions
                                //,
                                NodeName = nextStep.NodeName
                                ,
                                WorkFlowInstanceId = InstanceInfo.WorkFlowInstanceId
                                ,
                                WorkFlowProcessId = Guid.NewGuid()
                            };
                            if (stepOrder == 1)
                            {
                                wfp.StartTime = DateTime.Now;
                                wfp.StateCode = WorkFlowProcessState.Processing;
                                result.NextHandlerId = handlerIds;
                            }
                            wfpList.Add(wfp);
                        }
                        prevStep = wfpList.Find(n => n.StepOrder == stepOrder);
                        stepOrder++;
                    }
                    //获取下一节点
                    if (nextStep.Conditions.IsNotEmpty())
                    {
                        var stepConditions = new List<WorkFlowStepCondition>().DeserializeFromJson(nextStep.Conditions);
                        nextStep = null;//重设下一节点
                        //判断流转条件
                        foreach (var scnd in stepConditions)
                        {
                            var flag = true;
                            if (scnd.Conditions.NotEmpty())
                            {
                                foreach (var cnd in scnd.Conditions)
                                {
                                    if (cnd.CompareAttributeName.IsNotEmpty())
                                    {
                                        cnd.Values.Add(context.ObjectData.GetStringValue(cnd.CompareAttributeName));
                                    }
                                    var attr = _attributeFinder.Find(context.EntityMetaData.EntityId, cnd.AttributeName);
                                    flag = cnd.IsTrue(attr, context.ObjectData.GetStringValue(cnd.AttributeName));
                                    if (scnd.LogicalOperator == LogicalOperator.Or && flag)
                                    {
                                        break;
                                    }
                                    if (scnd.LogicalOperator == LogicalOperator.And && !flag)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (flag)
                            {
                                if (!scnd.NextStepId.Equals(Guid.Empty))
                                {
                                    nextStep = workFlowSteps.Find(n => n.WorkFlowStepId == scnd.NextStepId);
                                    hasNext = nextStep != null;
                                }
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        hasNext = nextStep != null;
                    }
                    else
                    {
                        nextStep = null;
                        hasNext = false;
                    }
                    //结束节点
                    if (hasNext && nextStep.NodeType == 1)
                    {
                        nextStep = null;
                        hasNext = false;
                        break;
                    }
                }
                if (wfpList.IsEmpty())
                {
                    result.IsSuccess = false;
                    result.Message = _loc["workflow_start_failure"];
                }
                result.Instance = InstanceInfo;
                result.Instance.Steps = wfpList;
            }
            OnStarting(context, result);
            _eventPublisher.Publish(new WorkFlowStartingEvent { Context = context, Result = result });
            if (result.IsSuccess)
            {
                try
                {
                    _WorkFlowInstanceService.BeginTransaction();
                    _WorkFlowInstanceService.Create(result.Instance);
                    _workFlowProcessService.CreateMany(result.Instance.Steps);

                    //更新记录流程状态
                    //_workFlowProcessUpdater.UpdateObjectProcessState(context.EntityMetaData, context.ObjectId, WorkFlowProcessState.Processing);
                    //上传附件
                    await _attachmentCreater.CreateManyAsync(context.EntityMetaData.EntityId, result.Instance.WorkFlowInstanceId, context.AttachmentFiles).ConfigureAwait(false);
                    _WorkFlowInstanceService.CompleteTransaction();
                    result.IsSuccess = true;
                }
                catch (Exception e)
                {
                    _WorkFlowInstanceService.RollBackTransaction();
                    result.IsSuccess = false;
                    result.Message = "error:" + e.Message;
                    _logService.Error(e);
                }
            }
            OnStarted(context, result);
            _eventPublisher.Publish(new WorkFlowStartedEvent { Context = context, Result = result });
            return result;
        }

        public virtual WorkFlowExecutionResult OnStarting(WorkFlowStartUpContext context, WorkFlowExecutionResult result)
        {
            return result;
        }

        public virtual WorkFlowExecutionResult OnStarted(WorkFlowStartUpContext context, WorkFlowExecutionResult result)
        {
            return result;
        }
    }
}