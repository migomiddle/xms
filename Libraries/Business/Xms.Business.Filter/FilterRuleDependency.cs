using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Business.Filter.Domain;
using Xms.Core.Data;
using Xms.Dependency;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;
using Xms.Schema.Attribute;

namespace Xms.Business.Filter
{
    /// <summary>
    /// 拦截规则依赖服务
    /// </summary>
    public class FilterRuleDependency : IFilterRuleDependency
    {
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDependencyService _dependencyService;

        public FilterRuleDependency(
            IAttributeFinder attributeFinder
            , IDependencyService dependencyService)
        {
            _attributeFinder = attributeFinder;
            _dependencyService = dependencyService;
        }

        public List<Schema.Domain.Attribute> GetRequireds(FilterRule entity)
        {
            if (entity.Conditions.IsNotEmpty())
            {
                //条件依赖于字段
                var ruleConditions = new FilterRuleConditions().DeserializeFromJson(entity.Conditions);
                var attributeNames = ruleConditions.Conditions.Select(s => s.AttributeName);
                return _attributeFinder.Query(x => x.Where(f => f.EntityId == entity.EntityId && f.Name.In(attributeNames)));
            }
            return null;
        }

        public bool Create(FilterRule entity)
        {
            bool result = true;
            var attributes = GetRequireds(entity);
            if (attributes.NotEmpty())
            {
                result = _dependencyService.Create(FilterRuleDefaults.ModuleName, entity.FilterRuleId, AttributeDefaults.ModuleName, attributes.Select(s => s.AttributeId).ToArray());
            }
            return result;
        }

        public bool Update(FilterRule entity)
        {
            bool result = true;
            var attributes = GetRequireds(entity);
            if (attributes.NotEmpty())
            {
                _dependencyService.Update(FilterRuleDefaults.ModuleName, entity.FilterRuleId, AttributeDefaults.ModuleName, attributes.Select(s => s.AttributeId).ToArray());
            }
            return result;
        }

        public bool Delete(params Guid[] id)
        {
            //删除依赖项
            return _dependencyService.DeleteByDependentId(FilterRuleDefaults.ModuleName, id);
        }
    }
}