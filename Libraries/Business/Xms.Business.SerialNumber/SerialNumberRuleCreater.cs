using System;
using Xms.Business.SerialNumber.Data;
using Xms.Context;
using Xms.Core;
using Xms.Data.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Localization.Abstractions;
using Xms.Plugin;
using Xms.Plugin.Domain;
using Xms.Solution;

namespace Xms.Business.SerialNumber
{
    /// <summary>
    /// 自动编号规则创建服务
    /// </summary>
    public class SerialNumberRuleCreater : ISerialNumberRuleCreater
    {
        private readonly ISerialNumberRuleRepository _serialNumberRuleRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly ILocalizedTextProvider _loc;
        private readonly ISolutionComponentService _solutionComponentService;
        private readonly ISerialNumberDependency _dependencyService;
        private readonly IEntityPluginCreater _entityPluginCreater;
        private readonly Caching.CacheManager<Domain.SerialNumberRule> _cacheService;
        private readonly IAppContext _appContext;

        public SerialNumberRuleCreater(IAppContext appContext
            , ISerialNumberRuleRepository serialNumberRuleRepository
            , ILocalizedLabelService localizedLabelService
            , ISolutionComponentService solutionComponentService
            , ISerialNumberDependency dependencyService
            , IEntityPluginCreater entityPluginCreater)
        {
            _appContext = appContext;
            _serialNumberRuleRepository = serialNumberRuleRepository;
            _loc = _appContext.GetFeature<ILocalizedTextProvider>();
            _localizedLabelService = localizedLabelService;
            _solutionComponentService = solutionComponentService;
            _dependencyService = dependencyService;
            _entityPluginCreater = entityPluginCreater;
            _cacheService = new Caching.CacheManager<Domain.SerialNumberRule>(SerialNumberRuleCache.CacheKey(_appContext), _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Create(Domain.SerialNumberRule entity)
        {
            //检查是否已存在相同字段的编码规则
            if (_serialNumberRuleRepository.Exists(x => x.EntityId == entity.EntityId && x.AttributeId == entity.AttributeId))
            {
                throw new XmsException(_loc["serial_number_duplicated"]);
            }
            entity.OrganizationId = this._appContext.OrganizationId;
            var result = true;
            using (UnitOfWork.Build(_serialNumberRuleRepository.DbContext))
            {
                result = _serialNumberRuleRepository.Create(entity);
                //依赖于字段
                _dependencyService.Create(entity);
                //solution component
                result = _solutionComponentService.Create(entity.SolutionId, entity.SerialNumberRuleId, SerialNumberRuleDefaults.ModuleName);
                //本地化标签
                _localizedLabelService.Create(entity.SolutionId, entity.Name.IfEmpty(""), SerialNumberRuleDefaults.ModuleName, "LocalizedName", entity.SerialNumberRuleId, this._appContext.BaseLanguage);
                _localizedLabelService.Create(entity.SolutionId, entity.Description.IfEmpty(""), SerialNumberRuleDefaults.ModuleName, "Description", entity.SerialNumberRuleId, this._appContext.BaseLanguage);
                //plugin
                _entityPluginCreater.Create(new EntityPlugin()
                {
                    AssemblyName = SerialNumberRuleDefaults.AssemblyName
                    ,
                    ClassName = SerialNumberRuleDefaults.PluginClassName
                    ,
                    EntityId = entity.EntityId
                    ,
                    EventName = Enum.GetName(typeof(OperationTypeEnum), OperationTypeEnum.Create)
                    ,
                    IsVisibled = false
                    ,
                    TypeCode = 0
                    ,
                    StateCode = RecordState.Enabled
                });
                //add to cache
                _cacheService.SetEntity(_serialNumberRuleRepository.FindById(entity.SerialNumberRuleId));
            }
            return result;
        }
    }
}