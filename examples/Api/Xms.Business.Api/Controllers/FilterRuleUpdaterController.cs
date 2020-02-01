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
    [Route("{org}/api/filterrule/update")]    
    public class FilterRuleUpdaterController : ApiCustomizeControllerBase
    {
        private readonly IFilterRuleCreater _filterRuleCreater;
        private readonly IFilterRuleUpdater _filterRuleUpdater;
        private readonly IFilterRuleFinder _filterRuleFinder;
        private readonly IFilterRuleDeleter _filterRuleDeleter;
        public FilterRuleUpdaterController(IWebAppContext appContext
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

        [Description("编辑拦截规则")]
        [HttpPost]
        public IActionResult Post(EditFilterRuleModel model)
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

        [Description("设置拦截规则可用状态")]
        [HttpPost("setstate")]
        public IActionResult SetFilterRuleState([FromBody]SetRecordStateModel model)
        {
            return _filterRuleUpdater.UpdateState(model.RecordId, model.IsEnabled).UpdateResult(T);
        }


    }
}
