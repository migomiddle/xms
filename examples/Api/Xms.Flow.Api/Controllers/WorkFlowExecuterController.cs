using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xms.Flow.Abstractions;
using Xms.Flow.Api.Models;
using Xms.Flow.Core;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Flow.Api
{
    /// <summary>
    /// 审批流执行接口
    /// </summary>
    [Route("{org}/api/workflow/execute")]
    public class WorkFlowExecuterController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IWorkFlowProcessFinder _workFlowProcessFinder;
        private readonly IWorkFlowInstanceService _workFlowInstanceService;
        private readonly IWorkFlowExecuter _workFlowExecuter;

        public WorkFlowExecuterController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IWorkFlowProcessFinder workFlowProcessFinder
            , IWorkFlowInstanceService workFlowInstanceService
            , IWorkFlowExecuter workFlowExecuter)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _workFlowProcessFinder = workFlowProcessFinder;
            _workFlowInstanceService = workFlowInstanceService;
            _workFlowExecuter = workFlowExecuter;
        }

        [HttpPost]
        [Description("审批处理")]
        public async Task<IActionResult> Post(WorkFlowProcessedModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ProcessState == WorkFlowProcessState.UnPassed && model.Description.IsEmpty())
                {
                    return JError(T["workflow_unpassed_needreason"]);
                }
                var processInfo = _workFlowProcessFinder.FindById(model.WorkFlowProcessId);
                if (processInfo == null)
                {
                    return NotFound();
                }
                if (processInfo.StateCode != WorkFlowProcessState.Processing)
                {
                    return JError(T["workflow_alreadyhandled"]);
                }
                if (processInfo.HandlerId != CurrentUser.SystemUserId)
                {
                    return JError(T["workflow_youarenothandler"]);
                }
                var instance = _workFlowInstanceService.FindById(processInfo.WorkFlowInstanceId);
                if (instance == null)
                {
                    return NotFound();
                }
                var entityMeta = _entityFinder.FindById(instance.EntityId);
                if (entityMeta == null)
                {
                    return NotFound();
                }
                //执行工作流
                var result = await _workFlowExecuter.ExecuteAsync(new WorkFlowExecutionContext()
                {
                    Attachments = model.Attachments != null ? model.Attachments.Count() : 0
                    ,
                    AttachmentFiles = model.Attachments
                    ,
                    Description = model.Description
                    ,
                    ProcessInfo = processInfo
                    ,
                    InstanceInfo = instance
                    ,
                    EntityMetaData = entityMeta
                    ,
                    ProcessState = model.ProcessState
                }).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    return JError(result.Message);
                }
                return JOk(T["operation_success"]);
            }
            return JError(T["operation_error"] + ": " + GetModelErrors(ModelState));
        }
    }
}