using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Context;
using Xms.Core;
using Xms.Core.Data;
using Xms.Identity;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Plugin.Abstractions;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Client;
using Xms.Sdk.Extensions;

namespace Xms.Business.DuplicateValidator
{
    /// <summary>
    /// 重复检测规则执行器
    /// </summary>
    public class DuplicateRuleExecutor : IDuplicateRuleExecutor, IEntityPlugin
    {
        private readonly IDuplicateRuleFinder _duplicateRuleFinder;
        private readonly IDuplicateRuleConditionService _duplicateRuleConditionService;
        private readonly IDataFinder _dataFinder;
        private readonly IAppContext _appContext;
        private readonly ILocalizedTextProvider _loc;

        public DuplicateRuleExecutor(IAppContext appContext
            , IDuplicateRuleFinder duplicateRuleFinder
            , IDuplicateRuleConditionService duplicateRuleConditionService
            , IDataFinder dataFinder)
        {
            _appContext = appContext;
            _loc = _appContext.GetFeature<ILocalizedTextProvider>();
            _duplicateRuleFinder = duplicateRuleFinder;
            _duplicateRuleConditionService = duplicateRuleConditionService;
            _dataFinder = dataFinder;
        }

        /// <summary>
        /// 重复规则命中
        /// </summary>
        /// <param name="organizationService"></param>
        /// <param name="data"></param>
        /// <param name="entityMetadata"></param>
        /// <param name="attributeMetadatas"></param>
        /// <returns></returns>
        public virtual IEnumerable<DuplicateRuleHitResult> ExecuteCore(Entity data, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas)
        {
            if (!entityMetadata.DuplicateEnabled)
            {
                yield break;
            }
            var duplicateRules = _duplicateRuleFinder.QueryByEntityId(entityMetadata.EntityId, RecordState.Enabled);
            if (duplicateRules.NotEmpty())
            {
                var languageId = _appContext.GetFeature<ICurrentUser>().UserSettings.LanguageId;
                foreach (var rule in duplicateRules)
                {
                    rule.Conditions = _duplicateRuleConditionService.Query(n => n.Where(w => w.DuplicateRuleId == rule.DuplicateRuleId));
                    var attrid = rule.Conditions.Select(s => s.AttributeId).ToArray();
                    var attributes = attributeMetadatas.Where(w => attrid.Contains(w.AttributeId));
                    QueryExpression qe = new QueryExpression(data.Name, languageId);
                    qe.AddColumns(attributes.Select(s => s.Name).ToArray());
                    FilterExpression filter = new FilterExpression(LogicalOperator.And);
                    foreach (var item in qe.ColumnSet.Columns)
                    {
                        if (!data.ContainsKey(item))
                        {
                            continue;
                        }
                        var attr = attributeMetadatas.Find(n => n.Name.IsCaseInsensitiveEqual(item));
                        object value = data.UnWrapAttributeValue(attr);
                        //忽略空值
                        if (rule.Conditions.Find(x => x.AttributeId == attr.AttributeId).IgnoreNullValues)
                        {
                            if (value == null || value.ToString().IsEmpty())
                            {
                                continue;
                            }
                        }
                        filter.AddCondition(item, ConditionOperator.NotNull);
                        filter.AddCondition(item, ConditionOperator.Equal, value);
                    }
                    if (filter.Conditions.NotEmpty() && data.ContainsKey(data.IdName))
                    {
                        //排除自身的比较
                        filter.AddCondition(data.IdName, ConditionOperator.NotEqual, data.GetIdValue());
                    }
                    if (filter.Conditions.NotEmpty())
                    {
                        qe.Criteria.AddFilter(filter);
                        var record = _dataFinder.Retrieve(qe, true);
                        if (record != null && record.Count > 0)
                        {
                            yield return new DuplicateRuleHitResult { Rule = rule, Target = record };
                        }
                    }
                }
            }
        }

        public void Execute(PluginExecutionContext context)
        {
            if (context.Stage == OperationStage.PreOperation)
            {
                var hitRules = ExecuteCore(context.Target, context.EntityMetadata, context.AttributeMetadatas);
                if (hitRules != null && hitRules.Any())
                {
                    string error = string.Empty;
                    foreach (var rule in hitRules)
                    {
                        error = " \n" + rule.Rule.Name + " \n" + rule.Rule.Description;
                    }
                    throw new XmsException(_loc["sdk_record_duplicated"] + ": " + error);
                }
            }
        }
    }
}