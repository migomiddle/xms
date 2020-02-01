using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xms.Api.Core.Controller;
using Xms.Business.Api.Models;
using Xms.Business.DataAnalyse.Visualization;
using Xms.Business.DuplicateValidator;
using Xms.Business.DuplicateValidator.Domain;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Framework.Context;
using Xms.Core.Data;

namespace Xms.Business.Api.Controllers
{
    [Route("{org}/api/duplicaterule")]
    [ApiController]
    public class DuplicateRuleController : ApiCustomizeControllerBase
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
        public IActionResult Get(DuplicateRuleModel model)
        {
            if (model.EntityId.Equals(Guid.Empty))
            {
                return NotFound();
            }
            var entity = _entityFinder.FindById(model.EntityId);
            if (entity == null)
            {
                return NotFound();
            }
            model.Entity = entity;
            FilterContainer<DuplicateRule> filter = FilterContainerBuilder.Build<DuplicateRule>();
            filter.And(n => n.EntityId == model.EntityId);
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
            return JOk(model);
        }

        [HttpGet("{id}")]
        [Description("数据重复检测规则编辑")]
        public IActionResult Get(Guid id)
        {
            EditDuplicateRuleModel model = new EditDuplicateRuleModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _duplicateRuleFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    model.Conditions = _duplicateRuleConditionService.Query(n => n.Where(w => w.DuplicateRuleId == entity.DuplicateRuleId));
                    return JOk(model);
                }
            }
            return NotFound();
        }
    }
}
