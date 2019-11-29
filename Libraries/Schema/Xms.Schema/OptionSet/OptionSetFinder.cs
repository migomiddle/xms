using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Dependency.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Module.Core;
using Xms.Schema.Abstractions;
using Xms.Schema.Data;

namespace Xms.Schema.OptionSet
{
    /// <summary>
    /// 选项集查询服务
    /// </summary>
    public class OptionSetFinder : IOptionSetFinder, IDependentLookup<Domain.OptionSet>
    {
        private readonly IOptionSetRepository _optionSetRepository;
        private readonly IOptionSetDetailFinder _optionSetDetailFinder;
        private readonly Caching.CacheManager<Domain.OptionSet> _cacheService;
        private readonly IAppContext _appContext;

        public OptionSetFinder(IAppContext appContext
            , IOptionSetRepository optionSetRepository
            , IOptionSetDetailFinder optionSetDetailFinder
            )
        {
            _appContext = appContext;
            _optionSetRepository = optionSetRepository;
            _optionSetDetailFinder = optionSetDetailFinder;
            _cacheService = new Caching.CacheManager<Domain.OptionSet>(_appContext.OrganizationUniqueName + ":optionsets", _appContext.PlatformSettings.CacheEnabled);
        }

        public Domain.OptionSet FindById(Guid id)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("optionsetid", id.ToString());

            Domain.OptionSet entity = _cacheService.Get(dic, () =>
             {
                 var o = _optionSetRepository.FindById(id);
                 if (o != null)
                 {
                     o.Items = _optionSetDetailFinder.FindByParentId(id);
                 }
                 return o;
             });

            if (entity != null)
            {
                WrapLocalizedLabel(entity);
            }
            return entity;
        }

        public Domain.OptionSet Find(Expression<Func<Domain.OptionSet, bool>> predicate)
        {
            var result = _optionSetRepository.Find(predicate);
            if (result != null)
            {
                result.Items = _optionSetDetailFinder.FindByParentId(result.OptionSetId);
                WrapLocalizedLabel(result);
            }
            return result;
        }

        public PagedList<Domain.OptionSet> QueryPaged(Func<QueryDescriptor<Domain.OptionSet>, QueryDescriptor<Domain.OptionSet>> container)
        {
            QueryDescriptor<Domain.OptionSet> q = container(QueryDescriptorBuilder.Build<Domain.OptionSet>());
            var datas = _optionSetRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public PagedList<Domain.OptionSet> QueryPaged(Func<QueryDescriptor<Domain.OptionSet>, QueryDescriptor<Domain.OptionSet>> container, Guid solutionId, bool existInSolution)
        {
            QueryDescriptor<Domain.OptionSet> q = container(QueryDescriptorBuilder.Build<Domain.OptionSet>());
            var datas = _optionSetRepository.QueryPaged(q, ModuleCollection.GetIdentity(OptionSetDefaults.ModuleName), solutionId, existInSolution);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<Domain.OptionSet> Query(Func<QueryDescriptor<Domain.OptionSet>, QueryDescriptor<Domain.OptionSet>> container)
        {
            QueryDescriptor<Domain.OptionSet> q = container(QueryDescriptorBuilder.Build<Domain.OptionSet>());
            var datas = _optionSetRepository.Query(q)?.ToList();

            WrapLocalizedLabel(datas);
            return datas;
        }

        public List<Domain.OptionSet> FindAll()
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

        private List<Domain.OptionSet> PreCacheAll()
        {
            var datas = _optionSetRepository.FindAll()?.ToList();
            if (datas.NotEmpty())
            {
                foreach (var o in datas)
                {
                    o.Items = _optionSetDetailFinder.FindByParentId(o.OptionSetId);
                }
            }
            return datas;
        }

        #region dependency

        public DependentDescriptor GetDependent(Guid dependentId)
        {
            var result = FindById(dependentId);
            return result != null ? new DependentDescriptor() { Name = result.Name } : null;
        }

        public int ComponentType => ModuleCollection.GetIdentity(OptionSetDefaults.ModuleName);

        #endregion dependency

        private void WrapLocalizedLabel(IEnumerable<Domain.OptionSet> datas)
        {
            /* 先关闭多语言切换功能 2018-10-10* /
            /*
            if (datas.NotEmpty())
            {
                var ids = datas.Select(f => f.OptionSetId);
                var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId.In(ids)));
                foreach (var d in datas)
                {
                    d.Name = _localizedLabelService.GetLabelText(labels, d.OptionSetId, "LocalizedName", d.Name);
                    d.Description = _localizedLabelService.GetLabelText(labels, d.OptionSetId, "Description", d.Description);
                }
            }
            */
        }

        private void WrapLocalizedLabel(Domain.OptionSet entity)
        {
            /* 先关闭多语言切换功能 2018-10-10* /
            /*
            var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId == entity.OptionSetId));
            entity.Name = _localizedLabelService.GetLabelText(labels, entity.OptionSetId, "LocalizedName", entity.Name);
            entity.Description = _localizedLabelService.GetLabelText(labels, entity.OptionSetId, "Description", entity.Description);
            */
        }
    }
}