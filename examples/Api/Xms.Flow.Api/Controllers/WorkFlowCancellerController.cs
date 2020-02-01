using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Flow.Core;
using Xms.Schema.Entity;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Flow.Api
{
    /// <summary>
    /// 审批流撤销接口
    /// </summary>
    [Route("{org}/api/workflow/cancel")]
    public class WorkFlowCancellerController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IWorkFlowCanceller _workFlowCanceller;

        public WorkFlowCancellerController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IWorkFlowCanceller workFlowCanceller)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _workFlowCanceller = workFlowCanceller;
        }

        [HttpPost]
        [Description("撤消审批")]
        public IActionResult Post(Guid entityid, Guid recordid)
        {
            WorkFlowCancellationContext context = new WorkFlowCancellationContext
            {
                EntityMetaData = _entityFinder.FindById(entityid)
                ,
                ObjectId = recordid
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