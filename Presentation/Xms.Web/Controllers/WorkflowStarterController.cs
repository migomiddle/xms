using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Flow;
using Xms.Flow.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Sdk.Client;
using Xms.Sdk.Extensions;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 审批流启动控制器
    /// </summary>
    [Route("{org}/flow/[action]")]
    public class WorkflowStarterController : AuthenticatedControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IWorkFlowFinder _workFlowFinder;
        private readonly IDataFinder _dataFinder;

        public WorkflowStarterController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IWorkFlowFinder workFlowFinder
            , IDataFinder dataFinder)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _workFlowFinder = workFlowFinder;
            _dataFinder = dataFinder;
        }

        [Description("启动审批")]
        [HttpGet]
        [HttpPost]
        public IActionResult StartWorkFlow(Guid entityId, Guid recordId)
        {
            StartWorkFlowModel model = new StartWorkFlowModel
            {
                EntityId = entityId,
                RecordId = recordId
            };
            //实体元数据
            var entityMetas = _entityFinder.FindById(model.EntityId);
            //查找记录
            var entity = _dataFinder.RetrieveById(entityMetas.Name, model.RecordId);
            if (entity.GetIntValue("ProcessState") == (int)WorkFlowProcessState.Processing)
            {
                return JError(T["workflow_processing_notallowtwice"]);
            }
            if (entity.GetIntValue("ProcessState") == (int)WorkFlowProcessState.Passed)
            {
                return JError(T["workflow_stop_notallowtwice"]);
            }
            if (entity.GetIntValue("ProcessState", -1) != -1
                && (entity.GetIntValue("ProcessState") == (int)WorkFlowProcessState.Waiting || entity.GetIntValue("ProcessState") == (int)WorkFlowProcessState.Disabled))
            {
                return JError(T["workflow_state_notallowtwice"]);
            }

            //找到审批流程
            var wfs = _workFlowFinder.QueryAuthorized(model.EntityId, FlowType.Approval);
            if (wfs.IsEmpty())
            {
                return NotFound();
            }
            model.WorkFlows = wfs;
            return View($"~/Views/Flow/{WebContext.ActionName}.cshtml", model);
        }
    }
}