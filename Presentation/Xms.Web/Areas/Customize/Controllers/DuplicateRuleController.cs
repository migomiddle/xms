using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Business.DuplicateValidator;
using Xms.Business.DuplicateValidator.Domain;
using Xms.Core;
using Xms.Core.Context;
using Xms.Core.Data;
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
    /// 重复检测规则管理控制器
    /// </summary>
    public class DuplicateRuleController : CustomizeBaseController
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IDuplicateRuleCreater _duplicateRuleCreater;
        private readonly IDuplicateRuleUpdater _duplicateRuleUpdater;
        private readonly IDuplicateRuleFinder _duplicateRuleFinder;
        private readonly IDuplicateRuleDeleter _duplicateRuleDeleter;
        private readonly IDuplicateRuleConditionService _duplicateRuleConditionService;

        public DuplicateRuleController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IDuplicateRuleCreater duplicateRuleCreater
            , IDuplicateRuleUpdater duplicateRuleUpdater
            , IDuplicateRuleFinder duplicateRuleFinder
            , IDuplicateRuleDeleter duplicateRuleDeleter
            , IDuplicateRuleConditionService duplicateRuleConditionService)
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _duplicateRuleCreater = duplicateRuleCreater;
            _duplicateRuleUpdater = duplicateRuleUpdater;
            _duplicateRuleFinder = duplicateRuleFinder;
            _duplicateRuleDeleter = duplicateRuleDeleter;
            _duplicateRuleConditionService = duplicateRuleConditionService;
        }

        [Description("数据重复检测规则列表")]
        public IActionResult Index(DuplicateRuleModel model)
        {
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }

            FilterContainer<DuplicateRule> filter = FilterContainerBuilder.Build<DuplicateRule>();
            if (!model.EntityId.Equals(Guid.Empty))
            {
                filter.And(n => n.EntityId == model.EntityId);
            }
            if (model.Name.IsNotEmpty())
            {
                filter.And(n => n.Name.Like(model.Name));
            }
            if (model.GetAll)
            {
                List<DuplicateRule> result = _duplicateRuleFinder.Query(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(filter)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

                model.Items = result;
                model.TotalItems = result.Count;
            }
            else
            {
                if (CurrentUser.UserSettings.PagingLimit > 0)
                {
                    model.PageSize = CurrentUser.UserSettings.PagingLimit;
                }
                PagedList<DuplicateRule> result = _duplicateRuleFinder.QueryPaged(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(filter)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

                model.Items = result.Items;
                model.TotalItems = result.TotalItems;
            }
            model.SolutionId = SolutionId.Value;
            return DynamicResult(model);
        }

        [HttpGet]
        [Description("新建数据重复检测规则")]
        public IActionResult CreateDuplicateRule(Guid entityid)
        {
            EditDuplicateRuleModel model = new EditDuplicateRuleModel
            {
                SolutionId = SolutionId.Value,
                EntityId = entityid,
                StateCode = RecordState.Enabled
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("新建数据重复检测规则-保存")]
        public IActionResult CreateDuplicateRule(EditDuplicateRuleModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new DuplicateRule();
                model.CopyTo(entity);
                entity.DuplicateRuleId = Guid.NewGuid();
                entity.CreatedBy = CurrentUser.SystemUserId;
                var conditions = new List<DuplicateRuleCondition>();
                int i = 0;
                foreach (var item in model.AttributeId)
                {
                    var cd = new DuplicateRuleCondition
                    {
                        DuplicateRuleConditionId = Guid.NewGuid(),
                        DuplicateRuleId = entity.DuplicateRuleId,
                        EntityId = model.EntityId,
                        IgnoreNullValues = model.IgnoreNullValues[i],
                        IsCaseSensitive = model.IsCaseSensitive[i],
                        AttributeId = item
                    };
                    conditions.Add(cd);
                    i++;
                }
                entity.Conditions = conditions;

                _duplicateRuleCreater.Create(entity);
                return CreateSuccess(new { id = entity.DuplicateRuleId });
            }
            var msg = GetModelErrors(ModelState);
            return CreateFailure(msg);
        }

        [HttpGet]
        [Description("数据重复检测规则编辑")]
        public IActionResult EditDuplicateRule(Guid id)
        {
            EditDuplicateRuleModel model = new EditDuplicateRuleModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _duplicateRuleFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    model.Conditions = _duplicateRuleConditionService.Query(n => n.Where(w => w.DuplicateRuleId == entity.DuplicateRuleId));
                    model.EntityMeta = _entityFinder.FindById(entity.EntityId);
                    return View(model);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("数据重复检测规则保存")]
        public IActionResult EditDuplicateRule(EditDuplicateRuleModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _duplicateRuleFinder.FindById(model.DuplicateRuleId.Value);
                entity.Description = model.Description;
                entity.Name = model.Name;
                entity.Intercepted = model.Intercepted;
                var conditions = _duplicateRuleConditionService.Query(n => n.Where(w => w.DuplicateRuleId == entity.DuplicateRuleId));
                int i = 0;
                entity.Conditions = new List<DuplicateRuleCondition>();
                foreach (var item in model.AttributeId)
                {
                    var id = model.DetailId[i];
                    var condition = new DuplicateRuleCondition
                    {
                        DuplicateRuleId = entity.DuplicateRuleId,
                        EntityId = model.EntityId,
                        IgnoreNullValues = model.IgnoreNullValues[i],
                        IsCaseSensitive = model.IsCaseSensitive[i],
                        AttributeId = item
                    };
                    if (id.Equals(Guid.Empty))
                    {
                        condition.DuplicateRuleConditionId = Guid.NewGuid();
                        _duplicateRuleConditionService.Create(condition);
                    }
                    else
                    {
                        condition.DuplicateRuleConditionId = id;
                        _duplicateRuleConditionService.Update(condition);
                        conditions.Remove(conditions.Find(n => n.DuplicateRuleConditionId == id));
                    }
                    entity.Conditions.Add(condition);

                    i++;
                }
                //delete lost detail
                var lostid = conditions.Select(n => n.DuplicateRuleConditionId).ToList();
                _duplicateRuleConditionService.DeleteById(lostid);

                _duplicateRuleUpdater.Update(entity);

                return UpdateSuccess(new { id = entity.DuplicateRuleId });
            }
            var msg = GetModelErrors(ModelState);
            return UpdateFailure(msg);
        }

        [Description("删除数据重复检测规则")]
        [HttpPost]
        public IActionResult DeleteDuplicateRule([FromBody]DeleteManyModel model)
        {
            return _duplicateRuleDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

        [Description("设置重复规则可用状态")]
        [HttpPost]
        public IActionResult SetDuplicateRuleState([FromBody]SetDuplicateRuleStateModel model)
        {
            return _duplicateRuleUpdater.UpdateState(model.RecordId, model.IsEnabled).UpdateResult(T);
        }
    }
}