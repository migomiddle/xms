using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Linq;
using Xms.Flow;
using Xms.Flow.Abstractions;
using Xms.Flow.Domain;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Client;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 审批流控制器
    /// </summary>
    public class FlowController : AuthenticatedControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IWorkFlowFinder _workFlowFinder;
        private readonly IWorkFlowProcessFinder _workFlowProcessFinder;
        private readonly IWorkFlowInstanceService _workFlowInstanceService;
        private readonly IWorkFlowProcessLogService _workFlowProcessLogService;
        private readonly IDataFinder _dataFinder;
        private readonly IWorkFlowStepService _workFlowStepService;

        public FlowController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IWorkFlowFinder workFlowFinder
            , IWorkFlowProcessFinder workFlowProcessFinder
            , IWorkFlowInstanceService workFlowInstanceService
            , IWorkFlowProcessLogService workFlowProcessLogService
            , IDataFinder dataFinder
            , IWorkFlowStepService workFlowStepService)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _workFlowFinder = workFlowFinder;
            _workFlowInstanceService = workFlowInstanceService;
            _workFlowProcessFinder = workFlowProcessFinder;
            _workFlowProcessLogService = workFlowProcessLogService;
            _dataFinder = dataFinder;
            _workFlowStepService = workFlowStepService;
        }

        #region 工作流

        [Description("任务移交")]
        [ValidateAntiForgeryToken]
        public IActionResult AssignHandler(Guid processId, string userName)
        {
            //查找用户
            var query = new QueryExpression("systemuser", CurrentUser.UserSettings.LanguageId);
            query.ColumnSet.AddColumns("systemuserid", "name");
            query.Criteria.FilterOperator = LogicalOperator.Or;
            query.Criteria.AddCondition("loginname", ConditionOperator.Equal, userName);
            query.Criteria.AddCondition("usernumber", ConditionOperator.Equal, userName);
            var user = _dataFinder.Retrieve(query);
            if (user == null || user.Count == 0)
            {
                return JError(T["workflow_nomatchuser"]);
            }
            Guid handlerId = user.GetIdValue();
            //当前步骤
            var processInfo = _workFlowProcessFinder.FindById(processId);
            if (processInfo == null)
            {
                return NotFound();
            }
            if (handlerId == CurrentUser.SystemUserId)
            {
                return JError(T["workflow_notallowtome"]);
            }
            var instance = _workFlowInstanceService.FindById(processInfo.WorkFlowInstanceId);
            if (handlerId == instance.ApplicantId)
            {
                return JError(T["workflow_notallowtoapplier"]);
            }
            //验证是否有移交权限
            //...
            var log = new WorkFlowProcessLog();
            log.CreatedOn = DateTime.Now;
            log.OperatorId = CurrentUser.SystemUserId;
            log.Title = T["workflow_assignto"];
            log.WorkFlowProcessId = Guid.Empty;
            log.WorkFlowProcessLogId = Guid.NewGuid();
            log.WorkFlowInstanceId = Guid.Empty;
            _workFlowProcessLogService.Create(log);
            return JOk(T["operation_success"]);
        }

        [Description("流程执行详情")]
        public IActionResult WorkFlowInstanceDetail(Guid entityid, Guid recordid)
        {
            var instances = _workFlowInstanceService.Query(n => n
                .Where(f => f.EntityId == entityid && f.ObjectId == recordid)
                .Sort(s => s.SortDescending(f => f.CreatedOn))
            );
            if (instances.IsEmpty())
            {
                return NotFound();
            }
            var entityMeta = _entityFinder.FindById(entityid);
            if (entityMeta == null)
            {
                return NotFound();
            }
            foreach (var instance in instances)
            {
                var steps = _workFlowProcessFinder.Query(n => n
                .Where(f => f.WorkFlowInstanceId == instance.WorkFlowInstanceId && f.StateCode != WorkFlowProcessState.Disabled)
                .Sort(s => s.SortAscending(f => f.StepOrder)).Sort(s => s.SortAscending(f => f.StateCode)));
                instance.Steps = steps;
            }
            WorkFlowInstanceDetailModel model = new WorkFlowInstanceDetailModel();
            model.Items = instances;
            var workFlowId = instances.First().WorkFlowId;
            model.FlowInfo = _workFlowFinder.FindById(workFlowId);

            var allSteps = _workFlowStepService.Query(n => n.Where(f => f.WorkFlowId == workFlowId).Sort(s => s.SortAscending(f => f.StepOrder)));

            model.Steps = allSteps;
            return View(model);
        }

        [Description("审批任务")]
        public IActionResult WorkFlowStateList(WorkFlowStateListModel model)
        {
            if (model.StateCode == 1)
            {
                var result = _workFlowProcessFinder.QueryHandlingList(CurrentUser.SystemUserId, model.Page, model.PageSize, model.EntityId);
                model.Items = result.Items;
                model.TotalItems = result.TotalItems;
            }
            else if (model.StateCode == 2)
            {
                var result = _workFlowProcessFinder.QueryHandledList(CurrentUser.SystemUserId, model.Page, model.PageSize, model.EntityId);
                model.Items = result.Items;
                model.TotalItems = result.TotalItems;
            }
            else if (model.StateCode == 0)
            {
                var result = _workFlowProcessFinder.QueryApplyHandlingList(CurrentUser.SystemUserId, model.Page, model.PageSize, model.EntityId);
                model.Items = result.Items;
                model.TotalItems = result.TotalItems;
            }
            model.HandledCount = _workFlowProcessFinder.QueryHandledCount(CurrentUser.SystemUserId, model.EntityId);
            model.HandlingCount = _workFlowProcessFinder.QueryHandlingCount(CurrentUser.SystemUserId, model.EntityId);
            model.ApplyHandlingCount = _workFlowProcessFinder.QueryApplyHandlingCount(CurrentUser.SystemUserId, model.EntityId);
            return DynamicResult(model);
        }

        #endregion 工作流
    }
}