using System;
using System.Linq;
using Xms.Business.DuplicateValidator.Domain;
using Xms.Dependency;
using Xms.Schema.Abstractions;

namespace Xms.Business.DuplicateValidator
{
    /// <summary>
    /// 重复检测规则依赖服务
    /// </summary>
    public class DuplicateRuleDependency : IDuplicateRuleDependency
    {
        private readonly IDependencyService _dependencyService;

        public DuplicateRuleDependency(IDependencyService dependencyService)
        {
            _dependencyService = dependencyService;
        }

        public bool Create(DuplicateRule entity)
        {
            //依赖于字段
            return _dependencyService.Create(DuplicateRuleDefaults.ModuleName, entity.DuplicateRuleId, AttributeDefaults.ModuleName, entity.Conditions.Select(x => x.AttributeId).ToArray());
        }

        public bool Update(DuplicateRule entity)
        {
            //依赖于字段
            return _dependencyService.Update(DuplicateRuleDefaults.ModuleName, entity.DuplicateRuleId, AttributeDefaults.ModuleName, entity.Conditions.Select(x => x.AttributeId).ToArray());
        }

        public bool Delete(params Guid[] id)
        {
            //删除依赖项
            return _dependencyService.DeleteByDependentId(DuplicateRuleDefaults.ModuleName, id);
        }
    }
}