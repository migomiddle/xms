using System;
using Xms.Dependency;
using Xms.Schema.Abstractions;

namespace Xms.Business.SerialNumber
{
    /// <summary>
    /// 自动编号规则依赖服务
    /// </summary>
    public class SerialNumberDependency : ISerialNumberDependency
    {
        private readonly IDependencyService _dependencyService;

        public SerialNumberDependency(IDependencyService dependencyService)
        {
            _dependencyService = dependencyService;
        }

        public bool Create(Domain.SerialNumberRule entity)
        {
            //依赖于字段
            return _dependencyService.Create(SerialNumberRuleDefaults.ModuleName, entity.SerialNumberRuleId, AttributeDefaults.ModuleName, entity.AttributeId);
        }

        public bool Update(Domain.SerialNumberRule entity)
        {
            //依赖于字段
            return _dependencyService.Update(SerialNumberRuleDefaults.ModuleName, entity.SerialNumberRuleId, AttributeDefaults.ModuleName, entity.AttributeId);
        }

        public bool Delete(params Guid[] id)
        {
            return _dependencyService.DeleteByDependentId(SerialNumberRuleDefaults.ModuleName, id); ;
        }
    }
}