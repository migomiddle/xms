using System;
using System.Linq;
using Xms.Business.Filter.Data;
using Xms.Business.Filter.Domain;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Plugin;

namespace Xms.Business.Filter
{
    /// <summary>
    /// 拦截规则删除服务
    /// </summary>
    public class FilterRuleDeleter : IFilterRuleDeleter, ICascadeDelete<Schema.Domain.Entity>
    {
        private readonly IFilterRuleRepository _filterRuleRepository;
        private readonly Caching.CacheManager<FilterRule> _cacheService;
        private readonly IFilterRuleDependency _dependencyService;
        private readonly IEntityPluginDeleter _entityPluginDeleter;
        private readonly IAppContext _appContext;

        public FilterRuleDeleter(IAppContext appContext
            , IFilterRuleRepository filterRuleRepository
            , IFilterRuleDependency dependencyService
            , IEntityPluginDeleter entityPluginDeleter)
        {
            _appContext = appContext;
            _filterRuleRepository = filterRuleRepository;
            _dependencyService = dependencyService;
            _entityPluginDeleter = entityPluginDeleter;
            _cacheService = new Caching.CacheManager<FilterRule>(FilterRuleCache.CacheKey(_appContext), _appContext.PlatformSettings.CacheEnabled);
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的实体</param>
        public void CascadeDelete(params Schema.Domain.Entity[] parent)
        {
            if (parent.IsEmpty())
            {
                return;
            }
            var entityIds = parent.Select(x => x.EntityId).ToArray();
            var deleteds = _filterRuleRepository.Query(x => x.EntityId.In(entityIds));
            if (deleteds.NotEmpty())
            {
                DeleteCore(deleteds.ToArray());
            }
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = false;
            var deleteds = _filterRuleRepository.Query(x => x.FilterRuleId.In(id));
            if (deleteds.NotEmpty())
            {
                result = DeleteCore(deleteds.ToArray());
            }
            return result;
        }

        private bool DeleteCore(params FilterRule[] deleteds)
        {
            Guard.NotEmpty(deleteds, nameof(deleteds));
            var result = false;
            var ids = deleteds.Select(x => x.FilterRuleId);
            using (UnitOfWork.Build(_filterRuleRepository.DbContext))
            {
                result = _filterRuleRepository.DeleteMany(ids);
                //删除依赖项
                _dependencyService.Delete(ids.ToArray());
                var entityIds = deleteds.Select(x => x.EntityId).Distinct().ToArray();
                //plugin
                foreach (var eid in entityIds)
                {
                    _entityPluginDeleter.DeleteByEntityId(eid);
                }
                foreach (var deleted in deleteds)
                {
                    //remove from cache
                    _cacheService.RemoveEntity(deleted);
                }
            }
            return result;
        }
    }
}