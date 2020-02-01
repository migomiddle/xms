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
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Business.Api.Controllers
{
    [Route("{org}/api/duplicaterule/creater")]    
    public class DuplicateRuleDeleterController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IDuplicateRuleCreater _duplicateRuleCreater;
        private readonly IDuplicateRuleUpdater _duplicateRuleUpdater;
        private readonly IDuplicateRuleFinder _duplicateRuleFinder;
        private readonly IDuplicateRuleDeleter _duplicateRuleDeleter;
        private readonly IDuplicateRuleConditionService _duplicateRuleConditionService;
        public DuplicateRuleDeleterController(IWebAppContext appContext
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

        [Description("删除数据重复检测规则")]
        [HttpPost]
        public IActionResult Post([FromBody]DeleteManyModel model)
        {
            return _duplicateRuleDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

    }
}
