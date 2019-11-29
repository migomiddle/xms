using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Core;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Flow;
using Xms.Flow.Core;
using Xms.Flow.Domain;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 流程管理控制器
    /// </summary>
    public class FlowController : CustomizeBaseController
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IWorkFlowFinder _workFlowFinder;
        private readonly IWorkFlowCreater _workFlowCreater;
        private readonly IWorkFlowUpdater _workFlowUpdater;
        private readonly IWorkFlowDeleter _workFlowDeleter;
        private readonly IWorkFlowInstanceService _workFlowInstanceService;
        private readonly IWorkFlowProcessFinder _workFlowProcessFinder;
        private readonly IProcessStageService _processStageService;
        private readonly IWorkFlowStepService _workFlowStepService;
        private readonly IWorkFlowCanceller _workFlowCanceller;

        public FlowController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , ISolutionService solutionService
            , IWorkFlowFinder workFlowFinder
            , IWorkFlowCreater workFlowCreater
            , IWorkFlowUpdater workFlowUpdater
            , IWorkFlowDeleter workFlowDeleter
            , IWorkFlowProcessFinder workFlowProcessFinder
            , IProcessStageService processStageService
            , IWorkFlowInstanceService workFlowInstanceService
            , IWorkFlowStepService workFlowStepService
            , IWorkFlowCanceller workFlowCanceller)
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _workFlowFinder = workFlowFinder;
            _workFlowCreater = workFlowCreater;
            _workFlowUpdater = workFlowUpdater;
            _workFlowDeleter = workFlowDeleter;
            _workFlowInstanceService = workFlowInstanceService;
            _workFlowProcessFinder = workFlowProcessFinder;
            _processStageService = processStageService;
            _workFlowStepService = workFlowStepService;
            _workFlowCanceller = workFlowCanceller;
        }

        [Description("流程列表")]
        public IActionResult Index(WorkFlowModel model)
        {
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }
            FilterContainer<WorkFlow> filter = FilterContainerBuilder.Build<WorkFlow>();
            if (model.Name.IsNotEmpty())
            {
                filter.And(n => n.Name.Like(model.Name));
            }

            if (model.GetAll)
            {
                model.Page = 1;
                model.PageSize = WebContext.PlatformSettings.MaxFetchRecords;
            }
            else if (!model.PageSizeBySeted && CurrentUser.UserSettings.PagingLimit > 0)
            {
                model.PageSize = CurrentUser.UserSettings.PagingLimit;
            }
            model.PageSize = model.PageSize > WebContext.PlatformSettings.MaxFetchRecords ? WebContext.PlatformSettings.MaxFetchRecords : model.PageSize;
            PagedList<WorkFlow> result = _workFlowFinder.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(filter)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                , SolutionId.Value, true);

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            model.SolutionId = SolutionId.Value;
            return DynamicResult(model);
        }

        [Description("设置流程可用状态")]
        [HttpPost]
        public IActionResult SetWorkFlowState([FromBody]SetRecordStateModel model)
        {
            if (model.RecordId.IsEmpty())
            {
                return NotSpecifiedRecord();
            }
            return _workFlowUpdater.UpdateState(model.IsEnabled, model.RecordId).UpdateResult(T);
        }

        [Description("设置流程权限启用状态")]
        [HttpPost]
        public IActionResult SetAuthorizationState([FromBody]SetFlowAuthorizationStateModel model)
        {
            if (model.RecordId.IsEmpty())
            {
                return NotSpecifiedRecord();
            }
            return _workFlowUpdater.UpdateAuthorization(model.IsAuthorization, model.RecordId).UpdateResult(T);
        }

        #region 审批流程

        [Description("新建流程")]
        public IActionResult CreateWorkFlow()
        {
            CreateWorkFlowModel model = new CreateWorkFlowModel
            {
                SolutionId = SolutionId.Value
            };
            return View(model);
        }

        [Description("新建流程")]
        [HttpPost]
        public IActionResult CreateWorkFlow([FromBody]CreateWorkFlowModel model)
        {
            if (model.StepData.IsEmpty())
            {
                ModelState.AddModelError("", T["workflow_step_empty"]);
            }
            if (ModelState.IsValid)
            {
                List<WorkFlowStep> steps = new List<WorkFlowStep>();
                steps = steps.DeserializeFromJson(model.StepData.UrlDecode());
                if (steps.IsEmpty())
                {
                    ModelState.AddModelError("", T["workflow_step_empty"]);
                }
                if (ModelState.IsValid)
                {
                    var entity = new WorkFlow();
                    model.CopyTo(entity);
                    entity.WorkFlowId = Guid.NewGuid();
                    entity.CreatedBy = CurrentUser.SystemUserId;
                    entity.CreatedOn = DateTime.Now;
                    entity.StateCode = RecordState.Enabled;
                    entity.SolutionId = SolutionId.Value;
                    entity.Category = 1;
                    foreach (var item in steps)
                    {
                        item.Conditions = item.Conditions.UrlDecode();
                        item.WorkFlowId = entity.WorkFlowId;
                    }
                    entity.Steps = steps;

                    _workFlowCreater.Create(entity);
                    _workFlowStepService.CreateMany(steps);
                    return CreateSuccess(new { id = entity.WorkFlowId });
                }
            }
            return CreateFailure(GetModelErrors());
        }

        [Description("编辑流程")]
        public IActionResult EditWorkFlow(Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return NotFound();
            }
            EditWorkFlowModel model = new EditWorkFlowModel();
            var entity = _workFlowFinder.FindById(id);
            if (entity == null)
            {
                return NotFound();
            }
            entity.CopyTo(model);
            model.Steps = _workFlowStepService.Query(n => n.Where(f => f.WorkFlowId == id).Sort(s => s.SortAscending(f => f.StepOrder)));
            model.StepData = model.Steps.SerializeToJson();
            model.EntityMetas = _entityFinder.FindById(entity.EntityId);

            return View(model);
        }

        [Description("编辑流程")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult EditWorkFlow([FromBody]EditWorkFlowModel model)
        {
            if (model.StepData.IsEmpty())
            {
                ModelState.AddModelError("", T["workflow_step_empty"]);
            }
            if (ModelState.IsValid)
            {
                model.StepData = model.StepData.UrlDecode();
                List<WorkFlowStep> steps = new List<WorkFlowStep>();
                steps = steps.DeserializeFromJson(model.StepData);
                if (steps.IsEmpty())
                {
                    ModelState.AddModelError("", T["workflow_step_empty"]);
                }
                if (ModelState.IsValid)
                {
                    var entity = _workFlowFinder.FindById(model.WorkFlowId);
                    model.EntityId = entity.EntityId;
                    model.CopyTo(entity);

                    _workFlowUpdater.Update(entity);
                    //steps
                    var orginalSteps = _workFlowStepService.Query(n => n.Where(f => f.WorkFlowId == model.WorkFlowId).Sort(s => s.SortAscending(f => f.StepOrder)));
                    foreach (var item in steps)
                    {
                        item.Conditions = item.Conditions.UrlDecode();
                        var old = orginalSteps.Find(n => n.WorkFlowStepId == item.WorkFlowStepId);
                        if (old == null)
                        {
                            item.WorkFlowId = entity.WorkFlowId;
                            _workFlowStepService.Create(item);
                        }
                        else
                        {
                            _workFlowStepService.Update(item);
                            orginalSteps.Remove(old);
                        }
                    }
                    if (orginalSteps.NotEmpty())
                    {
                        var lostid = orginalSteps.Select(n => n.WorkFlowStepId).ToArray();
                        _workFlowStepService.DeleteById(lostid);
                    }

                    return UpdateSuccess();
                }
            }
            return JError(GetModelErrors());
        }

        [Description("删除流程")]
        [HttpPost]
        public IActionResult DeleteWorkFlow([FromBody]DeleteManyModel model)
        {
            return _workFlowDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

        [Description("流程设计")]
        public IActionResult FlowDesign()
        {
            return View();
        }

        [Description("流程实例")]
        public IActionResult WorkFlowInstances(WorkFlowInstanceModel model)
        {
            if (model.WorkFlowId.Equals(Guid.Empty))
            {
                return NotFound();
            }
            model.FlowInfo = _workFlowFinder.FindById(model.WorkFlowId);
            FilterContainer<WorkFlowInstance> filter = FilterContainerBuilder.Build<WorkFlowInstance>();
            filter.And(n => n.WorkFlowId == model.WorkFlowId);

            if (model.GetAll)
            {
                model.Page = 1;
                model.PageSize = WebContext.PlatformSettings.MaxFetchRecords;
            }
            else if (!model.PageSizeBySeted && CurrentUser.UserSettings.PagingLimit > 0)
            {
                model.PageSize = CurrentUser.UserSettings.PagingLimit;
            }
            model.PageSize = model.PageSize > WebContext.PlatformSettings.MaxFetchRecords ? WebContext.PlatformSettings.MaxFetchRecords : model.PageSize;
            PagedList<WorkFlowInstance> result = _workFlowInstanceService.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(filter)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                );

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            return DynamicResult(model);
        }

        [Description("流程监控")]
        public IActionResult WorkFlowProcess(WorkFlowProcessModel model)
        {
            if (model.WorkFlowInstanceId.Equals(Guid.Empty))
            {
                return NotFound();
            }
            if (!model.IsSortBySeted)
            {
                model.SortBy = "steporder";
                model.SortDirection = 0;
            }
            model.InstanceInfo = _workFlowInstanceService.FindById(model.WorkFlowInstanceId);
            model.FlowInfo = _workFlowFinder.FindById(model.InstanceInfo.WorkFlowId);
            FilterContainer<WorkFlowProcess> filter = FilterContainerBuilder.Build<WorkFlowProcess>();
            filter.And(n => n.WorkFlowInstanceId == model.WorkFlowInstanceId);
            if (model.GetAll)
            {
                List<WorkFlowProcess> result = _workFlowProcessFinder.Query(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(filter)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

                model.Items = result;
                model.TotalItems = result.Count;
            }
            else
            {
                PagedList<WorkFlowProcess> result = _workFlowProcessFinder.QueryPaged(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(filter)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

                model.Items = result.Items;
                model.TotalItems = result.TotalItems;
            }
            return DynamicResult(model);
        }

        [HttpPost]
        [Description("流程撤销")]
        public IActionResult Cancel(Guid id)
        {
            var instance = _workFlowInstanceService.FindById(id);
            if (instance != null)
            {
                WorkFlowCancellationContext context = new WorkFlowCancellationContext
                {
                    EntityMetaData = _entityFinder.FindById(instance.EntityId)
                    ,
                    ObjectId = instance.ObjectId
                };
                var flag = _workFlowCanceller.Cancel(context);
                if (flag.IsSuccess)
                {
                    return JOk(T["operation_success"]);
                }
                else
                {
                    return JError(flag.Message);
                }
            }
            return JError(T["operation_error"]);
        }

        #endregion 审批流程

        #region 业务流程

        [Description("新建业务流程")]
        public IActionResult CreateBusinessFlow()
        {
            CreateBusinessFlowModel model = new CreateBusinessFlowModel
            {
                SolutionId = SolutionId.Value
            };
            return View(model);
        }

        [Description("新建业务流程")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult CreateBusinessFlow([FromBody]CreateWorkFlowModel model)
        {
            if (model.StepData.IsEmpty())
            {
                ModelState.AddModelError("", T["workflow_step_empty"]);
            }
            List<ProcessStage> steps = new List<ProcessStage>();
            steps = steps.DeserializeFromJson(model.StepData.UrlDecode());
            if (steps.IsEmpty())
            {
                ModelState.AddModelError("", T["workflow_step_empty"]);
            }
            if (ModelState.IsValid)
            {
                var entity = new WorkFlow();
                model.CopyTo(entity);
                entity.WorkFlowId = Guid.NewGuid();
                entity.Category = 2;
                entity.CreatedBy = CurrentUser.SystemUserId;
                entity.CreatedOn = DateTime.Now;
                entity.StateCode = Core.RecordState.Enabled;
                entity.SolutionId = SolutionId.Value;
                foreach (var item in steps)
                {
                    item.ProcessStageId = Guid.NewGuid();
                    item.WorkFlowId = entity.WorkFlowId;
                }

                _workFlowCreater.Create(entity);
                _processStageService.CreateMany(steps);
                return CreateSuccess(new { id = entity.WorkFlowId });
            }
            return CreateFailure(GetModelErrors());
        }

        [Description("编辑流程")]
        public IActionResult EditBusinessFlow(Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return NotFound();
            }
            EditBusinessFlowModel model = new EditBusinessFlowModel();
            var entity = _workFlowFinder.FindById(id);
            if (entity == null)
            {
                return NotFound();
            }
            entity.CopyTo(model);
            model.ProcessStages = _processStageService.Query(n => n.Where(f => f.WorkFlowId == id).Sort(s => s.SortAscending(f => f.StageOrder)));
            model.StepData = model.ProcessStages.SerializeToJson();

            return View(model);
        }

        [Description("编辑流程")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult EditBusinessFlow([FromBody]EditWorkFlowModel model)
        {
            string msg = string.Empty;
            if (model.StepData.IsEmpty())
            {
                ModelState.AddModelError("", T["workflow_step_empty"]);
            }
            model.StepData = model.StepData.UrlDecode();
            List<ProcessStage> steps = new List<ProcessStage>();
            steps = steps.DeserializeFromJson(model.StepData);
            if (steps.IsEmpty())
            {
                ModelState.AddModelError("", T["workflow_step_empty"]);
            }
            if (ModelState.IsValid)
            {
                var entity = _workFlowFinder.FindById(model.WorkFlowId);
                model.EntityId = entity.EntityId;
                model.StateCode = entity.StateCode;
                model.CopyTo(entity);

                _workFlowUpdater.Update(entity);
                var orginalSteps = _processStageService.Query(n => n.Where(f => f.WorkFlowId == model.WorkFlowId).Sort(s => s.SortAscending(f => f.StageOrder)));
                foreach (var item in steps)
                {
                    item.WorkFlowId = entity.WorkFlowId;
                    var old = orginalSteps.Find(n => n.ProcessStageId == item.ProcessStageId);
                    if (old == null)
                    {
                        _processStageService.Create(item);
                    }
                    else
                    {
                        _processStageService.Update(item);
                        orginalSteps.Remove(old);
                    }
                }
                if (orginalSteps.NotEmpty())
                {
                    var lostid = orginalSteps.Select(n => n.ProcessStageId).ToArray();
                    _processStageService.DeleteById(lostid);
                }
                return UpdateSuccess();
            }
            return UpdateFailure(GetModelErrors());
        }

        #endregion 业务流程
    }
}