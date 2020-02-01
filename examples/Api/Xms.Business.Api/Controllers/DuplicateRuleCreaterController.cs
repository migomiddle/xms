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
using Xms.Core;

namespace Xms.Business.Api.Controllers
{
    [Route("{org}/api/duplicaterule/creater")]    
    public class DuplicateRuleCreaterController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IDuplicateRuleCreater _duplicateRuleCreater;
        private readonly IDuplicateRuleUpdater _duplicateRuleUpdater;
        private readonly IDuplicateRuleFinder _duplicateRuleFinder;
        private readonly IDuplicateRuleDeleter _duplicateRuleDeleter;
        private readonly IDuplicateRuleConditionService _duplicateRuleConditionService;
        public DuplicateRuleCreaterController(IWebAppContext appContext
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

        [HttpGet]
        [Description("新建数据重复检测规则")]
        public IActionResult Get(Guid entityid)
        {
            EditDuplicateRuleModel model = new EditDuplicateRuleModel
            {
                SolutionId = SolutionId.Value,
                EntityId = entityid,
                StateCode = RecordState.Enabled
            };
            return JOk(model);
        }

        [HttpPost]        
        [Description("新建数据重复检测规则-保存")]
        public IActionResult Post(EditDuplicateRuleModel model)
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
                    var cd = new DuplicateRuleCondition();
                    cd.DuplicateRuleConditionId = Guid.NewGuid();
                    cd.DuplicateRuleId = entity.DuplicateRuleId;
                    cd.EntityId = model.EntityId;
                    cd.IgnoreNullValues = model.IgnoreNullValues[i];
                    cd.IsCaseSensitive = model.IsCaseSensitive[i];
                    cd.AttributeId = item;
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
    }
}
