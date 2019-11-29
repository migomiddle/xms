using System;
using System.Collections.Generic;
using Xms.Business.Filter.Domain;
using Xms.Core;
using Xms.Core.Data;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Plugin.Abstractions;
using Xms.Schema.Attribute;

namespace Xms.Business.Filter
{
    /// <summary>
    /// 拦截规则执行服务
    /// </summary>
    public class FilterRuleExecutor : IFilterRuleExecutor, IEntityPlugin
    {
        private readonly IFilterRuleFinder _filterRuleFinder;
        private readonly IAttributeFinder _attributeFinder;

        public FilterRuleExecutor(IFilterRuleFinder filterRuleFinder
            , IAttributeFinder attributeFinder)
        {
            _filterRuleFinder = filterRuleFinder;
            _attributeFinder = attributeFinder;
        }

        public void Execute(OperationTypeEnum op, Entity data, Schema.Domain.Entity entityMetadata)
        {
            List<FilterRule> rules = _filterRuleFinder.QueryByEntityId(entityMetadata.EntityId, Enum.GetName(typeof(OperationTypeEnum), op), RecordState.Enabled);

            if (rules.NotEmpty())
            {
                foreach (var rule in rules)
                {
                    if (rule.IsTrue(_attributeFinder, data))
                    {
                        throw new XmsException(rule.ToolTip);
                    }
                }
            }
        }

        public void Execute(PluginExecutionContext context)
        {
            if (context.Stage == OperationStage.PreOperation)
            {
                Execute(context.MessageName, context.Target, context.EntityMetadata);
            }
        }
    }
}