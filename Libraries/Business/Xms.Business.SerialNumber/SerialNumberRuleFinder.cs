using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Business.SerialNumber.Data;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Dependency.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Module.Core;

namespace Xms.Business.SerialNumber
{
    /// <summary>
    /// 自动编号规则查找服务
    /// </summary>
    public class SerialNumberRuleFinder : ISerialNumberRuleFinder, IDependentLookup<Domain.SerialNumberRule>
    {
        private readonly ISerialNumberRuleRepository _serialNumberRuleRepository;
        private readonly Caching.CacheManager<Domain.SerialNumberRule> _cacheService;
        private readonly IAppContext _appContext;

        public SerialNumberRuleFinder(IAppContext appContext
            , ISerialNumberRuleRepository serialNumberRuleRepository
            )
        {
            _appContext = appContext;
            _serialNumberRuleRepository = serialNumberRuleRepository;
            _cacheService = new Caching.CacheManager<Domain.SerialNumberRule>(SerialNumberRuleCache.CacheKey(_appContext), _appContext.PlatformSettings.CacheEnabled);
        }

        public Domain.SerialNumberRule FindById(Guid id)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("SerialNumberRuleId", id.ToString());
            Domain.SerialNumberRule entity = _cacheService.Get(dic, () =>
             {
                 return _serialNumberRuleRepository.FindById(id);
             });
            return entity;
        }

        public Domain.SerialNumberRule FindByEntityId(Guid entityid)
        {
            List<Domain.SerialNumberRule> entities = _cacheService.GetVersionItems(entityid.ToString(), () =>
            {
                return this.Query(n => n.Where(x => x.EntityId == entityid));
            });
            if (entities.NotEmpty())
            {
                return entities.SingleOrDefault(x => x.StateCode == Core.RecordState.Enabled);
            }
            return null;
        }

        public Domain.SerialNumberRule Find(Expression<Func<Domain.SerialNumberRule, bool>> predicate)
        {
            var data = _serialNumberRuleRepository.Find(predicate);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public PagedList<Domain.SerialNumberRule> QueryPaged(Func<QueryDescriptor<Domain.SerialNumberRule>, QueryDescriptor<Domain.SerialNumberRule>> container)
        {
            QueryDescriptor<Domain.SerialNumberRule> q = container(QueryDescriptorBuilder.Build<Domain.SerialNumberRule>());
            var datas = _serialNumberRuleRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public PagedList<Domain.SerialNumberRule> QueryPaged(Func<QueryDescriptor<Domain.SerialNumberRule>, QueryDescriptor<Domain.SerialNumberRule>> container, Guid solutionId, bool existInSolution)
        {
            QueryDescriptor<Domain.SerialNumberRule> q = container(QueryDescriptorBuilder.Build<Domain.SerialNumberRule>());
            var datas = _serialNumberRuleRepository.QueryPaged(q, Module.Core.ModuleCollection.GetIdentity(SerialNumberRuleDefaults.ModuleName), solutionId, existInSolution);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<Domain.SerialNumberRule> Query(Func<QueryDescriptor<Domain.SerialNumberRule>, QueryDescriptor<Domain.SerialNumberRule>> container)
        {
            QueryDescriptor<Domain.SerialNumberRule> q = container(QueryDescriptorBuilder.Build<Domain.SerialNumberRule>());
            var datas = _serialNumberRuleRepository.Query(q)?.ToList();

            WrapLocalizedLabel(datas);
            return datas;
        }

        public List<Domain.SerialNumberRule> FindAll()
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

        private List<Domain.SerialNumberRule> PreCacheAll()
        {
            return _serialNumberRuleRepository.FindAll()?.ToList();
        }

        #region dependency

        public DependentDescriptor GetDependent(Guid dependentId)
        {
            var result = FindById(dependentId);
            return result != null ? new DependentDescriptor() { Name = result.Name } : null;
        }

        public int ComponentType => ModuleCollection.GetIdentity(SerialNumberRuleDefaults.ModuleName);

        #endregion dependency

        private void WrapLocalizedLabel(IEnumerable<Domain.SerialNumberRule> datas)
        {
            //if (datas.NotEmpty())
            //{
            //    var ids = datas.Select(f => f.SerialNumberRuleId);
            //    var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId.In(ids)));
            //    foreach (var d in datas)
            //    {
            //        d.Name = _localizedLabelService.GetLabelText(labels, d.SerialNumberRuleId, "LocalizedName", d.Name);
            //        d.Description = _localizedLabelService.GetLabelText(labels, d.SerialNumberRuleId, "Description", d.Description);
            //    }
            //}
        }

        private void WrapLocalizedLabel(Domain.SerialNumberRule entity)
        {
            //var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId == entity.SerialNumberRuleId));
            //entity.Name = _localizedLabelService.GetLabelText(labels, entity.SerialNumberRuleId, "LocalizedName", entity.Name);
            //entity.Description = _localizedLabelService.GetLabelText(labels, entity.SerialNumberRuleId, "Description", entity.Description);
        }
    }
}