using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Xms.Flow;
using Xms.Flow.Core;
using Xms.Schema.Entity;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Web.Api
{
    /// <summary>
    /// 审批流撤销接口
    /// </summary>
    [Route("{org}/api/workflow/cancel")]
    public class WorkFlowCancellerController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IWorkFlowInstanceService _workFlowInstanceService;
        private readonly IWorkFlowCanceller _workFlowCanceller;

        public WorkFlowCancellerController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IWorkFlowInstanceService workFlowInstanceService
            , IWorkFlowCanceller workFlowCanceller)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _workFlowInstanceService = workFlowInstanceService;
            _workFlowCanceller = workFlowCanceller;
        }

        [HttpPost("{id}")]
        [Description("撤消审批")]
        public IActionResult Post([FromRoute]WorkFlowInstanceCancleModel model)
        {
            var flowInstance = _workFlowInstanceService.FindById(model.Id);
            if (flowInstance == null)
            {
                return NotFound();
            }
            WorkFlowCancellationContext context = new WorkFlowCancellationContext
            {
                EntityMetaData = _entityFinder.FindById(flowInstance.EntityId)
                ,
                ObjectId = flowInstance.ObjectId
            };
            var result = _workFlowCanceller.Cancel(context);
            if (result.IsSuccess)
            {
                return JOk(T["operation_success"]);
            }
            return JError(result.Message);
        }

        [HttpPost]
        [Description("撤消审批")]
        public IActionResult Post(WorkFlowCancelModel model)
        {
            WorkFlowCancellationContext context = new WorkFlowCancellationContext
            {
                EntityMetaData = _entityFinder.FindById(model.EntityId)
                ,
                ObjectId = model.RecordId
            };
            var result = _workFlowCanceller.Cancel(context);
            if (result.IsSuccess)
            {
                return JOk(T["operation_success"]);
            }
            return JError(result.Message);
        }
    }
}