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
    [Route("{org}/api/filterrule/delete")]    
    public class FilterRuleDeleterController : ApiCustomizeControllerBase
    {
        private readonly IFilterRuleCreater _filterRuleCreater;
        private readonly IFilterRuleUpdater _filterRuleUpdater;
        private readonly IFilterRuleFinder _filterRuleFinder;
        private readonly IFilterRuleDeleter _filterRuleDeleter;
        public FilterRuleDeleterController(IWebAppContext appContext
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

        [Description("删除拦截规则")]
        [HttpPost]
        public IActionResult Post([FromBody]DeleteManyModel model)
        {
            return _filterRuleDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

    }
}
