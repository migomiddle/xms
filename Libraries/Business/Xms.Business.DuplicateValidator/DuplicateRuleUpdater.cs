using System;
using System.Collections.Generic;
using Xms.Business.DuplicateValidator.Data;
using Xms.Business.DuplicateValidator.Domain;
using Xms.Context;
using Xms.Core;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Localization;

namespace Xms.Business.DuplicateValidator
{
    /// <summary>
    /// 重复检测规则更新服务
    /// </summary>
    public class DuplicateRuleUpdater : IDuplicateRuleUpdater
    {
        private readonly IDuplicateRuleRepository _duplicateRuleRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IDuplicateRuleDependency _dependencyService;
        private readonly Caching.CacheManager<DuplicateRule> _cacheService;
        private readonly IAppContext _appContext;

        public DuplicateRuleUpdater(IAppContext appContext
            , IDuplicateRuleRepository duplicateRuleRepository
            , ILocalizedLabelService localizedLabelService
            , IDuplicateRuleDependency dependencyService)
        {
            _appContext = appContext;
            _duplicateRuleRepository = duplicateRuleRepository;
            _localizedLabelService = localizedLabelService;
            _dependencyService = dependencyService;
            _cacheService = new Caching.CacheManager<DuplicateRule>(_appContext.OrganizationUniqueName + ":duplicaterules", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Update(DuplicateRule entity)
        {
            var result = false;
            using (UnitOfWork.Build(_duplicateRuleRepository.DbContext))
            {
                result = _duplicateRuleRepository.Update(entity);
                //依赖于字段
                _dependencyService.Update(entity);
                //localization
                _localizedLabelService.Update(entity.Name.IfEmpty(""), "LocalizedName", entity.DuplicateRuleId, _appContext.BaseLanguage);
                _localizedLabelService.Update(entity.Description.IfEmpty(""), "Description", entity.DuplicateRuleId, _appContext.BaseLanguage);
                //set to cache
                _cacheService.SetEntity(entity);
            }
            return result;
        }

        public bool UpdateState(IEnumerable<Guid> ids, bool isEnabled)
        {
            var context = UpdateContextBuilder.Build<DuplicateRule>();
            context.Set(f => f.StateCode, isEnabled ? RecordState.Enabled : RecordState.Disabled);
            context.Where(f => f.DuplicateRuleId.In(ids));
            var flag = _duplicateRuleRepository.Update(context);
            if (flag)
            {
                var updated = _duplicateRuleRepository.Query(x => x.DuplicateRuleId.In(ids));
                foreach (var item in updated)
                {
                    //set to cache
                    _cacheService.SetEntity(item);
                }
            }
            return flag;
        }
    }
}