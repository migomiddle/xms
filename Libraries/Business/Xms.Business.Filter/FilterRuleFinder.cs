using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Business.Filter.Data;
using Xms.Business.Filter.Domain;
using Xms.Context;
using Xms.Core;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Dependency.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Module.Core;

namespace Xms.Business.Filter
{
    /// <summary>
    /// 拦截规则查找服务
    /// </summary>
    public class FilterRuleFinder : IFilterRuleFinder, IDependentLookup<FilterRule>
    {
        private readonly IFilterRuleRepository _filterRuleRepository;
        private readonly Caching.CacheManager<FilterRule> _cacheService;
        private readonly IAppContext _appContext;

        public FilterRuleFinder(IAppContext appContext
            , IFilterRuleRepository filterRuleRepository)
        {
            _appContext = appContext;
            _filterRuleRepository = filterRuleRepository;
            _cacheService = new Caching.CacheManager<FilterRule>(FilterRuleCache.CacheKey(_appContext), _appContext.PlatformSettings.CacheEnabled);
        }

        public FilterRule FindById(Guid id)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("FilterRuleId", id.ToString());
            FilterRule entity = _cacheService.Get(dic, () =>
             {
                 return _filterRuleRepository.FindById(id);
             }
            );
            return entity;
        }

        public List<FilterRule> QueryByEntityId(Guid entityid, string eventName, RecordState? recordState)
        {
            List<FilterRule> entities = _cacheService.GetVersionItems(entityid + "/*/" + eventName + "/", () =>
             {
                 if (recordState.HasValue)
                 {
                     return this.Query(n => n.Where(f => f.EntityId == entityid && f.EventName == eventName && f.StateCode == recordState.Value));
                 }
                 return this.Query(n => n.Where(f => f.EntityId == entityid && f.EventName == eventName));
             });
            if (recordState.HasValue && entities.NotEmpty())
            {
                entities.RemoveAll(x => x.StateCode != recordState);
            }
            return entities;
        }

        public PagedList<FilterRule> QueryPaged(Func<QueryDescriptor<FilterRule>, QueryDescriptor<FilterRule>> container)
        {
            QueryDescriptor<FilterRule> q = container(QueryDescriptorBuilder.Build<FilterRule>());

            return _filterRuleRepository.QueryPaged(q);
        }

        public PagedList<FilterRule> QueryPaged(Func<QueryDescriptor<FilterRule>, QueryDescriptor<FilterRule>> container, Guid solutionId, bool existInSolution)
        {
            QueryDescriptor<FilterRule> q = container(QueryDescriptorBuilder.Build<FilterRule>());
            var datas = _filterRuleRepository.QueryPaged(q, Module.Core.ModuleCollection.GetIdentity(FilterRuleDefaults.ModuleName), solutionId, existInSolution);

            return datas;
        }

        public List<FilterRule> Query(Func<QueryDescriptor<FilterRule>, QueryDescriptor<FilterRule>> container)
        {
            QueryDescriptor<FilterRule> q = container(QueryDescriptorBuilder.Build<FilterRule>());

            return _filterRuleRepository.Query(q)?.ToList();
        }

        public List<FilterRule> FindAll()
        {
            var entities = _cacheService.GetVersionItems("all", () =>
             {
                 return PreCacheAll();
             });
            return entities;
        }

        private List<FilterRule> PreCacheAll()
        {
            return _filterRuleRepository.FindAll()?.ToList();
        }

        #region dependency

        public DependentDescriptor GetDependent(Guid dependentId)
        {
            var result = FindById(dependentId);
            return result != null ? new DependentDescriptor() { Name = result.Name } : null;
        }

        public int ComponentType => ModuleCollection.GetIdentity(FilterRuleDefaults.ModuleName);

        #endregion dependency
    }
}