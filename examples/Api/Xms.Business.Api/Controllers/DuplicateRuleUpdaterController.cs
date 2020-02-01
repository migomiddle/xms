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
using Xms.Web.Framework.Mvc;

namespace Xms.Business.Api.Controllers
{
    [Route("{org}/api/duplicaterule/updater")]    
    public class DuplicateRuleUpdaterController : ApiCustomizeControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IDuplicateRuleCreater _duplicateRuleCreater;
        private readonly IDuplicateRuleUpdater _duplicateRuleUpdater;
        private readonly IDuplicateRuleFinder _duplicateRuleFinder;
        private readonly IDuplicateRuleDeleter _duplicateRuleDeleter;
        private readonly IDuplicateRuleConditionService _duplicateRuleConditionService;
        public DuplicateRuleUpdaterController(IWebAppContext appContext
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

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Description("数据重复检测规则保存")]
        public IActionResult Post(EditDuplicateRuleModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _duplicateRuleFinder.FindById(model.DuplicateRuleId.Value);
                entity.Description = model.Description;
                entity.Name = model.Name;
                entity.Intercepted = model.Intercepted;
                _duplicateRuleUpdater.Update(entity);
                var conditions = _duplicateRuleConditionService.Query(n => n.Where(w => w.DuplicateRuleId == entity.DuplicateRuleId));
                int i = 0;
                foreach (var item in model.AttributeId)
                {
                    var id = model.DetailId[i];
                    var condition = new DuplicateRuleCondition();
                    condition.DuplicateRuleId = entity.DuplicateRuleId;
                    condition.EntityId = model.EntityId;
                    condition.IgnoreNullValues = model.IgnoreNullValues[i];
                    condition.IsCaseSensitive = model.IsCaseSensitive[i];
                    condition.AttributeId = item;
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

                    i++;
                }
                //delete lost detail
                var lostid = conditions.Select(n => n.DuplicateRuleConditionId).ToList();
                _duplicateRuleConditionService.DeleteById(lostid);

                return UpdateSuccess(new { id = entity.DuplicateRuleId });
            }
            var msg = GetModelErrors(ModelState);
            return UpdateFailure(msg);
        }

        [Description("设置重复规则可用状态")]
        [HttpPost("setstate")]
        public IActionResult SetDuplicateRuleState([FromBody]SetDuplicateRuleStateModel model)
        {
            return _duplicateRuleUpdater.UpdateState(model.RecordId, model.IsEnabled).UpdateResult(T);
        }
    }
}
