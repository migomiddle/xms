using System;
using System.Collections.Generic;
using Xms.Business.DuplicateValidator.Domain;
using Xms.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Business.DuplicateValidator
{
    /// <summary>
    /// 重复检测规则导入服务
    /// </summary>
    [SolutionImportNode("duplicaterules")]
    public class DuplicateRuleImporter : ISolutionComponentImporter<DuplicateRule>
    {
        private readonly IDuplicateRuleCreater _duplicateRuleCreater;
        private readonly IDuplicateRuleUpdater _duplicateRuleUpdater;
        private readonly IDuplicateRuleFinder _duplicateRuleFinder;
        private readonly IDuplicateRuleConditionService _duplicateRuleConditionService;
        private readonly IAppContext _appContext;

        public DuplicateRuleImporter(IAppContext appContext
            , IDuplicateRuleCreater duplicateRuleCreater
            , IDuplicateRuleUpdater duplicateRuleUpdater
            , IDuplicateRuleFinder duplicateRuleFinder
            , IDuplicateRuleConditionService duplicateRuleConditionService)
        {
            _appContext = appContext;
            _duplicateRuleCreater = duplicateRuleCreater;
            _duplicateRuleUpdater = duplicateRuleUpdater;
            _duplicateRuleFinder = duplicateRuleFinder;
            _duplicateRuleConditionService = duplicateRuleConditionService;
        }

        public bool Import(Guid solutionId, IList<DuplicateRule> duplicateRules)
        {
            if (duplicateRules.NotEmpty())
            {
                foreach (var item in duplicateRules)
                {
                    var entity = _duplicateRuleFinder.FindById(item.DuplicateRuleId);
                    if (entity != null)
                    {
                        _duplicateRuleUpdater.Update(item);
                        if (item.Conditions.NotEmpty())
                        {
                            _duplicateRuleConditionService.DeleteByParentId(entity.DuplicateRuleId);
                            foreach (var cnd in item.Conditions)
                            {
                                cnd.DuplicateRuleId = item.DuplicateRuleId;
                            }
                            _duplicateRuleConditionService.CreateMany(item.Conditions);
                        }
                    }
                    else
                    {
                        item.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        item.SolutionId = solutionId;
                        item.ComponentState = 0;
                        item.CreatedOn = DateTime.Now;
                        _duplicateRuleCreater.Create(item);
                    }
                }
            }
            return true;
        }
    }
}