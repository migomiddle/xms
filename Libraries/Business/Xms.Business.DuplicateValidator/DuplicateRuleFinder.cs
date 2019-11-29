using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Business.DuplicateValidator.Data;
using Xms.Business.DuplicateValidator.Domain;
using Xms.Context;
using Xms.Core;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Dependency.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Module.Core;
using Xms.Solution;

namespace Xms.Business.DuplicateValidator
{
    /// <summary>
    /// 重复检测规则查找服务
    /// </summary>
    public class DuplicateRuleFinder : IDuplicateRuleFinder, IDependentLookup<DuplicateRule>
    {
        private readonly IDuplicateRuleRepository _duplicateRuleRepository;

        //private readonly ILocalizedLabelService _localizedLabelService;
        private readonly ISolutionComponentService _solutionComponentService;

        private readonly Caching.CacheManager<DuplicateRule> _cacheService;
        private readonly IAppContext _appContext;

        public DuplicateRuleFinder(IAppContext appContext
            , IDuplicateRuleRepository duplicateRuleRepository
            , ISolutionComponentService solutionComponentService)
        {
            _appContext = appContext;
            _duplicateRuleRepository = duplicateRuleRepository;
            _solutionComponentService = solutionComponentService;
            _cacheService = new Caching.CacheManager<DuplicateRule>(DuplicateRuleCache.CacheKey(_appContext), _appContext.PlatformSettings.CacheEnabled);
        }

        public DuplicateRule FindById(Guid id)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("DuplicateRuleId", id.ToString());
            DuplicateRule entity = _cacheService.Get(dic, () =>
             {
                 return _duplicateRuleRepository.FindById(id);
             });
            if (entity != null)
            {
                WrapLocalizedLabel(entity);
            }
            return entity;
        }

        public List<DuplicateRule> QueryByEntityId(Guid entityid, RecordState? state)
        {
            string sindex = entityid.ToString() + "/" + (state.HasValue ? "" : state.Value.ToString());
            List<DuplicateRule> entities = _cacheService.GetVersionItems(sindex, () =>
            {
                if (state.HasValue)
                {
                    return this.Query(n => n.Where(f => f.EntityId == entityid && f.StateCode == state.Value));
                }
                return this.Query(n => n.Where(f => f.EntityId == entityid));
            });
            if (entities.NotEmpty())
            {
                if (state.HasValue)
                {
                    entities.RemoveAll(x => x.StateCode != state.Value);
                }
            }
            return entities;
        }

        public PagedList<DuplicateRule> QueryPaged(Func<QueryDescriptor<DuplicateRule>, QueryDescriptor<DuplicateRule>> container)
        {
            QueryDescriptor<DuplicateRule> q = container(QueryDescriptorBuilder.Build<DuplicateRule>());

            var datas = _duplicateRuleRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public PagedList<DuplicateRule> QueryPaged(Func<QueryDescriptor<DuplicateRule>, QueryDescriptor<DuplicateRule>> container, Guid solutionId, bool existInSolution)
        {
            QueryDescriptor<DuplicateRule> q = container(QueryDescriptorBuilder.Build<DuplicateRule>());
            var datas = _duplicateRuleRepository.QueryPaged(q, Module.Core.ModuleCollection.GetIdentity(DuplicateRuleDefaults.ModuleName), solutionId, existInSolution);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<DuplicateRule> Query(Func<QueryDescriptor<DuplicateRule>, QueryDescriptor<DuplicateRule>> container)
        {
            QueryDescriptor<DuplicateRule> q = container(QueryDescriptorBuilder.Build<DuplicateRule>());
            var datas = _duplicateRuleRepository.Query(q)?.ToList();

            WrapLocalizedLabel(datas);
            return datas;
        }

        public List<DuplicateRule> FindAll()
        {
            var entities = _cacheService.GetVersionItems("all", () =>
             {
                 return PreCacheAll();
             });
            if (entities != null)
            {
                WrapLocalizedLabel(entities);
            }
            return entities;
        }

        private List<DuplicateRule> PreCacheAll()
        {
            return _duplicateRuleRepository.FindAll()?.ToList();
        }

        #region dependency

        public DependentDescriptor GetDependent(Guid dependentId)
        {
            var result = FindById(dependentId);
            return result != null ? new DependentDescriptor() { Name = result.Name } : null;
        }

        public int ComponentType => ModuleCollection.GetIdentity(DuplicateRuleDefaults.ModuleName);

        #endregion dependency

        private void WrapLocalizedLabel(IEnumerable<DuplicateRule> datas)
        {
            //if (datas.NotEmpty())
            //{
            //    var ids = datas.Select(f => f.DuplicateRuleId);
            //    var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this._appContext.CurrentUser.UserSettings.LanguageId && f.ObjectId.In(ids)));
            //    foreach (var d in datas)
            //    {
            //        d.Name = _localizedLabelService.GetLabelText(labels, d.DuplicateRuleId, "LocalizedName", d.Name);
            //        d.Description = _localizedLabelService.GetLabelText(labels, d.DuplicateRuleId, "Description", d.Description);
            //    }
            //}
        }

        private void WrapLocalizedLabel(DuplicateRule entity)
        {
            //var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this._appContext.CurrentUser.UserSettings.LanguageId && f.ObjectId == entity.DuplicateRuleId));
            //entity.Name = _localizedLabelService.GetLabelText(labels, entity.DuplicateRuleId, "LocalizedName", entity.Name);
            //entity.Description = _localizedLabelService.GetLabelText(labels, entity.DuplicateRuleId, "Description", entity.Description);
        }
    }
}