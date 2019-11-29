using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Business.Filter;
using Xms.Business.Filter.Domain;
using Xms.Core.Context;
using Xms.Data.Provider;
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
    /// 拦截规则管理控制器
    /// </summary>
    public class FilterRuleController : CustomizeBaseController
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IFilterRuleCreater _filterRuleCreater;
        private readonly IFilterRuleUpdater _filterRuleUpdater;
        private readonly IFilterRuleFinder _filterRuleFinder;
        private readonly IFilterRuleDeleter _filterRuleDeleter;

        public FilterRuleController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IFilterRuleCreater filterRuleCreater
            , IFilterRuleUpdater filterRuleUpdater
            , IFilterRuleFinder filterRuleFinder
            , IFilterRuleDeleter filterRuleDeleter)
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _filterRuleCreater = filterRuleCreater;
            _filterRuleUpdater = filterRuleUpdater;
            _filterRuleFinder = filterRuleFinder;
            _filterRuleDeleter = filterRuleDeleter;
        }

        [Description("拦截规则列表")]
        public IActionResult Index(FilterRulesModel model)
        {
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }
            FilterContainer<FilterRule> filter = FilterContainerBuilder.Build<FilterRule>();
            if (!model.EntityId.Equals(Guid.Empty))
            {
                filter.And(n => n.EntityId == model.EntityId);
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
            PagedList<FilterRule> result = _filterRuleFinder.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(filter)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection)));
            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            return DynamicResult(model);
        }

        [HttpGet]
        [Description("新建拦截规则")]
        public IActionResult CreateFilterRule(Guid entityid)
        {
            CreateFilterRuleModel model = new CreateFilterRuleModel();
            model.EntityId = entityid;
            return View(model);
        }

        [Description("新建拦截规则")]
        [HttpPost]
        public IActionResult CreateFilterRule(CreateFilterRuleModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new FilterRule();
                model.CopyTo(entity);
                entity.CreatedBy = CurrentUser.SystemUserId;
                entity.FilterRuleId = Guid.NewGuid();
                var flag = _filterRuleCreater.Create(entity);
                if (flag)
                {
                    return JOk(T["created_success"], new { id = entity.FilterRuleId });
                }
                return JError(T["created_error"]);
            }
            return JError(T["created_error"] + ": " + GetModelErrors());
        }

        [HttpGet]
        [Description("编辑拦截规则")]
        public IActionResult EditFilterRule(Guid id)
        {
            EditFilterRuleModel model = new EditFilterRuleModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _filterRuleFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    model.FilterRuleId = id;
                    model.Name = entity.Name;
                    model.EventName = entity.EventName;
                    model.EntityId = entity.EntityId;
                    model.ToolTip = entity.ToolTip;
                    model.StateCode = entity.StateCode;
                    model.EntityMeta = _entityFinder.FindById(entity.EntityId);
                    return View(model);
                }
            }
            return NotFound();
        }

        [Description("编辑拦截规则")]
        [HttpPost]
        public IActionResult EditFilterRule(EditFilterRuleModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _filterRuleFinder.FindById(model.FilterRuleId);
                entity.Name = model.Name;
                entity.Conditions = model.Conditions;
                entity.EventName = model.EventName;
                entity.ModifiedBy = CurrentUser.SystemUserId;
                entity.ModifiedOn = DateTime.Now;
                entity.StateCode = model.StateCode;
                return _filterRuleUpdater.Update(entity).UpdateResult(T);
            }
            return JError(T["saved_error"] + ": " + GetModelErrors());
        }

        [Description("删除拦截规则")]
        [HttpPost]
        public IActionResult DeleteFilterRule([FromBody]DeleteManyModel model)
        {
            return _filterRuleDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

        [Description("设置拦截规则可用状态")]
        [HttpPost]
        public IActionResult SetFilterRuleState([FromBody]SetRecordStateModel model)
        {
            return _filterRuleUpdater.UpdateState(model.RecordId, model.IsEnabled).UpdateResult(T);
        }
    }
}