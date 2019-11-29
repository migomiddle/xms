using System;
using System.Collections.Generic;
using Xms.Business.Filter.Data;
using Xms.Business.Filter.Domain;
using Xms.Context;
using Xms.Core;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Data.Provider;
using Xms.Schema.Attribute;

namespace Xms.Business.Filter
{
    /// <summary>
    /// 拦截规则更新服务
    /// </summary>
    public class FilterRuleUpdater : IFilterRuleUpdater
    {
        private readonly IFilterRuleRepository _filterRuleRepository;
        private readonly Caching.CacheManager<FilterRule> _cacheService;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IFilterRuleDependency _dependencyService;
        private readonly IAppContext _appContext;

        public FilterRuleUpdater(IAppContext appContext
            , IFilterRuleRepository filterRuleRepository
            , IAttributeFinder attributeFinder
            , IFilterRuleDependency dependencyService)
        {
            _appContext = appContext;
            _filterRuleRepository = filterRuleRepository;
            _attributeFinder = attributeFinder;
            _dependencyService = dependencyService;
            _cacheService = new Caching.CacheManager<FilterRule>(FilterRuleCache.CacheKey(_appContext), _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Update(FilterRule entity)
        {
            var result = false;
            using (UnitOfWork.Build(_filterRuleRepository.DbContext))
            {
                result = _filterRuleRepository.Update(entity);
                //依赖
                _dependencyService.Update(entity);
                //set to cache
                _cacheService.SetEntity(_filterRuleRepository.FindById(entity.FilterRuleId));
            }
            return result;
        }

        public bool UpdateState(IEnumerable<Guid> ids, bool isEnabled)
        {
            var context = UpdateContextBuilder.Build<FilterRule>();
            context.Set(f => f.StateCode, isEnabled ? RecordState.Enabled : RecordState.Disabled);
            context.Where(f => f.FilterRuleId.In(ids));
            var result = false;
            using (UnitOfWork.Build(_filterRuleRepository.DbContext))
            {
                result = _filterRuleRepository.Update(context);
                var items = _filterRuleRepository.Query(x => x.FilterRuleId.In(ids));
                foreach (var item in items)
                {
                    _cacheService.SetListItem(item);
                }
            }
            return result;
        }
    }
}