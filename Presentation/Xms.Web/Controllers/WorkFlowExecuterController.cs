using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Linq;
using Xms.Flow;
using Xms.Flow.Domain;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Sdk.Client;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 审批流执行控制器
    /// </summary>
    [Route("{org}/flow/[action]")]
    public class WorkFlowExecuterController : AuthenticatedControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IWorkFlowProcessService _workFlowProcessService;
        private readonly IWorkFlowProcessFinder _workFlowProcessFinder;
        private readonly IWorkFlowInstanceService _workFlowInstanceService;
        private readonly IWorkFlowExecuter _workFlowExecuter;
        private readonly IDataFinder _dataFinder;

        public WorkFlowExecuterController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IWorkFlowProcessService workFlowProcessService
            , IWorkFlowProcessFinder workFlowProcessFinder
            , IWorkFlowInstanceService workFlowInstanceService
            , IWorkFlowExecuter workFlowExecuter
            , IDataFinder dataFinder)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _workFlowProcessService = workFlowProcessService;
            _workFlowProcessFinder = workFlowProcessFinder;
            _workFlowInstanceService = workFlowInstanceService;
            _workFlowExecuter = workFlowExecuter;
            _dataFinder = dataFinder;
        }

        [Description("审批处理")]
        public IActionResult WorkFlowProcessing(WorkFlowProcessingModel model)
        {
            if (model.EntityId.Equals(Guid.Empty) || model.RecordId.Equals(Guid.Empty))
            {
                return NotFound();
            }
            var entityMetas = _entityFinder.FindById(model.EntityId);
            var entity = _dataFinder.RetrieveById(entityMetas.Name, model.RecordId);
            var instances = _workFlowInstanceService.Query(n => n.Take(1).Where(f => f.EntityId == model.EntityId && f.ObjectId == model.RecordId).Sort(s => s.SortDescending(f => f.CreatedOn)));
            WorkFlowInstance instance = null;
            if (instances.NotEmpty())
            {
                instance = instances.First();
            }
            if (instance == null)
            {
                return NotFound();
            }
            var processInfo = _workFlowProcessFinder.GetCurrentStep(instance.WorkFlowInstanceId, CurrentUser.SystemUserId);
            if (processInfo == null)
            {
                if (_workFlowProcessFinder.GetLastHandledStep(instance.WorkFlowInstanceId, CurrentUser.SystemUserId) != null)
                {
                    return JError("您已处理");
                }
                return JError(T["workflow_nopermission"]);
            }
            model.InstanceInfo = instance;
            model.ProcessInfo = processInfo;
            model.ProcessList = _workFlowProcessFinder.Query(n => n.Where(f => f.WorkFlowInstanceId == instance.WorkFlowInstanceId).Sort(s => s.SortAscending(f => f.StepOrder)));

            return View($"~/Views/Flow/{WebContext.ActionName}.cshtml", model);
        }
    }
}