using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Data.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.QueryView.Abstractions;
using Xms.QueryView.Data;
using Xms.Schema.Attribute;
using Xms.Solution.Abstractions;

namespace Xms.QueryView
{
    /// <summary>
    /// 视图创建服务
    /// </summary>
    public class QueryViewCreater : IQueryViewCreater
    {
        private readonly IQueryViewRepository _queryViewRepository;
        private readonly IDefaultAttributeProvider _defaultAttributeProvider;
        private readonly ILocalizedLabelBatchBuilder _localizedLabelService;
        private readonly IQueryViewDependency _dependencyService;
        private readonly IDefaultQueryViewProvider _defaultQueryViewProvider;
        private readonly Caching.CacheManager<Domain.QueryView> _cacheService;
        private readonly IAppContext _appContext;

        public QueryViewCreater(IAppContext appContext
            , IQueryViewRepository queryViewRepository
            , IDefaultAttributeProvider defaultAttributeProvider
            , ILocalizedLabelBatchBuilder localizedLabelService
            , IQueryViewDependency dependencyService
            , IDefaultQueryViewProvider defaultQueryViewProvider)
        {
            _appContext = appContext;
            _queryViewRepository = queryViewRepository;
            _localizedLabelService = localizedLabelService;
            _defaultAttributeProvider = defaultAttributeProvider;
            _dependencyService = dependencyService;
            _defaultQueryViewProvider = defaultQueryViewProvider;
            _cacheService = new Caching.CacheManager<Domain.QueryView>(_appContext.OrganizationUniqueName + ":queryviews", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Create(Domain.QueryView entity)
        {
            return CreateCore(entity, (n) =>
            {
                _dependencyService.Create(n);
            });
        }

        public bool CreateDefaultView(Schema.Domain.Entity entity)
        {
            var (DefaultView, Dependents) = _defaultQueryViewProvider.Get(entity, _defaultAttributeProvider.GetSysAttributes(entity));
            return this.Create(DefaultView);
        }

        private bool CreateCore(Domain.QueryView entity, Action<Domain.QueryView> createDependents)
        {
            entity.SolutionId = SolutionDefaults.DefaultSolutionId;//组件属于默认解决方案
            var result = true;
            using (UnitOfWork.Build(_queryViewRepository.DbContext))
            {
                result = _queryViewRepository.Create(entity);
                //依赖项
                createDependents(entity);

                //本地化标签
                _localizedLabelService.Append(entity.SolutionId, entity.Name.IfEmpty(""), QueryViewDefaults.ModuleName, "LocalizedName", entity.QueryViewId)
                .Append(entity.SolutionId, entity.Description.IfEmpty(""), QueryViewDefaults.ModuleName, "Description", entity.QueryViewId)
                .Save();
                //add to cache
                _cacheService.SetEntity(entity);
            }
            return result;
        }

        private void WrapLocalizedLabel(List<Domain.QueryView> datas)
        {
            /* 先关闭多语言切换功能 2018-10-10* /
            /*
            if (datas.NotEmpty())
            {
                var ids = datas.Select(f => f.QueryViewId);
                var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId.In(ids)));
                foreach (var d in datas)
                {
                    d.Name = _localizedLabelService.GetLabelText(labels, d.QueryViewId, "LocalizedName", d.Name);
                    d.Description = _localizedLabelService.GetLabelText(labels, d.QueryViewId, "Description", d.Description);
                }
            }
            */
        }

        private void WrapLocalizedLabel(Domain.QueryView entity)
        {
            /* 先关闭多语言切换功能 2018-10-10* /
            /*
            var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId == entity.QueryViewId));
            entity.Name = _localizedLabelService.GetLabelText(labels, entity.QueryViewId, "LocalizedName", entity.Name);
            entity.Description = _localizedLabelService.GetLabelText(labels, entity.QueryViewId, "Description", entity.Description);
            */
        }
    }
}