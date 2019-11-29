using System;
using System.Collections.Generic;
using Xms.Business.SerialNumber.Data;
using Xms.Context;
using Xms.Core;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Localization.Abstractions;

namespace Xms.Business.SerialNumber
{
    /// <summary>
    /// 自动编号规则更新服务
    /// </summary>
    public class SerialNumberRuleUpdater : ISerialNumberRuleUpdater
    {
        private readonly ISerialNumberRuleRepository _serialNumberRuleRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly ILocalizedTextProvider _loc;
        private readonly ISerialNumberDependency _dependencyService;
        private readonly Caching.CacheManager<Domain.SerialNumberRule> _cacheService;
        private readonly IAppContext _appContext;

        public SerialNumberRuleUpdater(IAppContext appContext
            , ISerialNumberRuleRepository serialNumberRuleRepository
            , ILocalizedLabelService localizedLabelService
            , ISerialNumberDependency dependencyService)
        {
            _appContext = appContext;
            _serialNumberRuleRepository = serialNumberRuleRepository;
            _loc = _appContext.GetFeature<ILocalizedTextProvider>();
            _localizedLabelService = localizedLabelService;
            _dependencyService = dependencyService;
            _cacheService = new Caching.CacheManager<Domain.SerialNumberRule>(SerialNumberRuleCache.CacheKey(_appContext), _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Update(Domain.SerialNumberRule entity)
        {
            //检查是否已存在相同字段的编码规则
            if (_serialNumberRuleRepository.Exists(x => x.SerialNumberRuleId != entity.SerialNumberRuleId && x.EntityId == entity.EntityId && x.AttributeId == entity.AttributeId))
            {
                throw new XmsException(_loc["serial_number_duplicated"]);
            }
            bool result = _serialNumberRuleRepository.Update(entity);
            if (result)
            {
                //依赖于字段
                _dependencyService.Update(entity);
                //localization
                _localizedLabelService.Update(entity.Name.IfEmpty(""), "LocalizedName", entity.SerialNumberRuleId, this._appContext.BaseLanguage);
                _localizedLabelService.Update(entity.Description.IfEmpty(""), "Description", entity.SerialNumberRuleId, this._appContext.BaseLanguage);
                //set to cache
                _cacheService.SetEntity(_serialNumberRuleRepository.FindById(entity.SerialNumberRuleId));
            }
            return result;
        }

        public bool UpdateState(IEnumerable<Guid> ids, bool isEnabled)
        {
            var context = UpdateContextBuilder.Build<Domain.SerialNumberRule>();
            context.Set(f => f.StateCode, isEnabled ? RecordState.Enabled : RecordState.Disabled);
            context.Where(f => f.SerialNumberRuleId.In(ids));
            var flag = _serialNumberRuleRepository.Update(context);
            if (flag)
            {
                //set to cache
                var items = _serialNumberRuleRepository.Query(x => x.SerialNumberRuleId.In(ids));
                foreach (var item in items)
                {
                    _cacheService.SetEntity(item);
                }
            }
            return flag;
        }
    }
}