using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Api.Core.Controller;
using Xms.Business.Api.Models;
using Xms.Business.Filter;
using Xms.Business.Filter.Domain;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Solution;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;


namespace Xms.Business.Api.Controllers
{
    [Route("{org}/api/filterrule")]    
    public class FilterRuleController : ApiCustomizeControllerBase
    {
        private readonly IFilterRuleCreater _filterRuleCreater;
        private readonly IFilterRuleUpdater _filterRuleUpdater;
        private readonly IFilterRuleFinder _filterRuleFinder;
        private readonly IFilterRuleDeleter _filterRuleDeleter;
        public FilterRuleController(IWebAppContext appContext
            , ISolutionService solutionService
            , IFilterRuleCreater filterRuleCreater
            , IFilterRuleUpdater filterRuleUpdater
            , IFilterRuleFinder filterRuleFinder
            , IFilterRuleDeleter filterRuleDeleter)
            : base(appContext, solutionService)
        {
            _filterRuleCreater = filterRuleCreater;
            _filterRuleUpdater = filterRuleUpdater;
            _filterRuleFinder = filterRuleFinder;
            _filterRuleDeleter = filterRuleDeleter;
        }

        [Description("拦截规则列表")]
        public IActionResult Get(FilterRulesModel model)
        {
            FilterContainer<FilterRule> filter = FilterContainerBuilder.Build<FilterRule>();
            //filter.And(n => n.SolutionId == this.solutionId.Value);
            if (!model.EntityId.Equals(Guid.Empty))
            {
                filter.And(n => n.EntityId == model.EntityId);
            }
            if (model.GetAll)
            {
                model.Page = 1;
                model.PageSize = 25000;
            }
            else if (CurrentUser.UserSettings.PagingLimit > 0)
            {
                model.PageSize = CurrentUser.UserSettings.PagingLimit;
            }
            PagedList<FilterRule> result = _filterRuleFinder.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(filter)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection)));
            model.Items = result.Items;
            model.TotalItems = result.TotalItems;            
            return JOk(model);
        }

        [HttpGet("{id}")]
        [Description("编辑拦截规则")]
        public IActionResult Get(Guid id)
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
                    return JOk(model);
                }
            }
            return NotFound();
        }

    }
}
