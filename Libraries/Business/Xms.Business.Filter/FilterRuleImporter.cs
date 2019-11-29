using System;
using System.Collections.Generic;
using Xms.Business.Filter.Domain;
using Xms.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Business.Filter
{
    /// <summary>
    /// 拦截规则导入服务
    /// </summary>
    [SolutionImportNode("filterrules")]
    public class FilterRuleImporter : ISolutionComponentImporter<FilterRule>
    {
        private readonly IFilterRuleCreater _filterRuleCreater;
        private readonly IFilterRuleUpdater _filterRuleUpdater;
        private readonly IFilterRuleFinder _filterRuleFinder;
        private readonly IAppContext _appContext;

        public FilterRuleImporter(IAppContext appContext
            , IFilterRuleCreater filterRuleCreater
            , IFilterRuleUpdater filterRuleUpdater
            , IFilterRuleFinder filterRuleFinder)
        {
            _appContext = appContext;
            _filterRuleCreater = filterRuleCreater;
            _filterRuleUpdater = filterRuleUpdater;
            _filterRuleFinder = filterRuleFinder;
        }

        public bool Import(Guid solutionId, IList<FilterRule> businessRules)
        {
            if (businessRules.NotEmpty())
            {
                foreach (var item in businessRules)
                {
                    var entity = _filterRuleFinder.FindById(item.FilterRuleId);
                    if (entity == null)
                    {
                        item.ComponentState = 0;
                        item.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        item.CreatedOn = DateTime.Now;
                        _filterRuleCreater.Create(item);
                    }
                    else
                    {
                        _filterRuleUpdater.Update(item);
                    }
                }
            }
            return true;
        }
    }
}