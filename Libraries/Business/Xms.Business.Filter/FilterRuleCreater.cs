using Xms.Business.Filter.Data;
using Xms.Business.Filter.Domain;
using Xms.Context;
using Xms.Core;
using Xms.Data.Abstractions;
using Xms.Plugin;
using Xms.Plugin.Domain;
using Xms.Schema.Attribute;

namespace Xms.Business.Filter
{
    /// <summary>
    /// 拦截规则创建服务
    /// </summary>
    public class FilterRuleCreater : IFilterRuleCreater
    {
        private readonly IFilterRuleRepository _filterRuleRepository;
        private readonly Caching.CacheManager<FilterRule> _cacheService;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IFilterRuleDependency _dependencyService;
        private readonly IEntityPluginCreater _entityPluginCreater;
        private readonly IAppContext _appContext;

        public FilterRuleCreater(IAppContext appContext
            , IFilterRuleRepository filterRuleRepository
            , IAttributeFinder attributeFinder
            , IFilterRuleDependency dependencyService
            , IEntityPluginCreater entityPluginCreater)
        {
            _appContext = appContext;
            _filterRuleRepository = filterRuleRepository;
            _attributeFinder = attributeFinder;
            _dependencyService = dependencyService;
            _entityPluginCreater = entityPluginCreater;
            _cacheService = new Caching.CacheManager<FilterRule>(FilterRuleCache.CacheKey(_appContext), _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Create(FilterRule entity)
        {
            bool result = false;
            using (UnitOfWork.Build(_filterRuleRepository.DbContext))
            {
                result = _filterRuleRepository.Create(entity);
                //依赖
                _dependencyService.Create(entity);
                //plugin
                _entityPluginCreater.Create(new EntityPlugin()
                {
                    AssemblyName = FilterRuleDefaults.AssemblyName
                    ,
                    ClassName = FilterRuleDefaults.PluginClassName
                    ,
                    EntityId = entity.EntityId
                    ,
                    EventName = entity.EventName
                    ,
                    IsVisibled = false
                    ,
                    TypeCode = 0
                    ,
                    StateCode = RecordState.Enabled
                });
                //set to cache
                _cacheService.SetEntity(_filterRuleRepository.FindById(entity.FilterRuleId));
            }
            return result;
        }
    }
}