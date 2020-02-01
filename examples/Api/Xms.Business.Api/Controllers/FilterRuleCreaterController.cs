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
    [Route("{org}/api/filterrule/create")]    
    public class FilterRuleCreaterController : ApiCustomizeControllerBase
    {
        private readonly IFilterRuleCreater _filterRuleCreater;
        private readonly IFilterRuleUpdater _filterRuleUpdater;
        private readonly IFilterRuleFinder _filterRuleFinder;
        private readonly IFilterRuleDeleter _filterRuleDeleter;
        public FilterRuleCreaterController(IWebAppContext appContext
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

        [Description("新建拦截规则")]
        [HttpGet]
        public IActionResult Get(Guid entityid)
        {
            CreateFilterRuleModel model = new CreateFilterRuleModel();
            model.EntityId = entityid;
            return JOk(model);
        }

        [Description("新建拦截规则")]
        [HttpPost]
        public IActionResult Post(CreateFilterRuleModel model)
        {
            string msg = string.Empty;
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
            msg = GetModelErrors(ModelState);
            return JError(T["created_error"] + ": " + msg);
        }


    }
}
