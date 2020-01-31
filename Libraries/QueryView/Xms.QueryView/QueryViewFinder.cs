using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Dependency.Abstractions;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Module.Core;
using Xms.QueryView.Abstractions;
using Xms.QueryView.Data;

namespace Xms.QueryView
{
    /// <summary>
    /// 视图查询服务
    /// </summary>
    public class QueryViewFinder : IQueryViewFinder, IDependentLookup<Domain.QueryView>
    {
        private readonly IQueryViewRepository _queryViewRepository;
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly Caching.CacheManager<Domain.QueryView> _cacheService;
        private readonly IAppContext _appContext;
        private readonly ICurrentUser _currentUser;

        public QueryViewFinder(IAppContext appContext
            , IQueryViewRepository queryViewRepository
            , IRoleObjectAccessService roleObjectAccessService
            )
        {
            _appContext = appContext;
            _currentUser = _appContext.GetFeature<ICurrentUser>();
            _queryViewRepository = queryViewRepository;
            _roleObjectAccessService = roleObjectAccessService;
            _cacheService = new Caching.CacheManager<Domain.QueryView>(_appContext.OrganizationUniqueName + ":queryviews", _appContext.PlatformSettings.CacheEnabled);
        }

        public Domain.QueryView FindById(Guid id)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("QueryViewId", id.ToString());
            Domain.QueryView entity = _cacheService.Get(dic, () =>
             {
                 return _queryViewRepository.FindById(id);
             });
            if (entity != null)
            {
                WrapLocalizedLabel(entity);
            }
            return entity;
        }

        public Domain.QueryView FindEntityDefaultView(Guid entityId)
        {
            List<Domain.QueryView> views = _cacheService.GetVersionItems(entityId.ToString(), () =>
             {
                 return this.QueryAuthorized(n => n.Where(f => f.EntityId == entityId));
             });
            Domain.QueryView defaultView = null;
            if (views.NotEmpty())
            {
                defaultView = views.FirstOrDefault(n => n.IsDefault == true);
            }
            if (defaultView != null)
            {
                WrapLocalizedLabel(defaultView);
            }
            return defaultView;
        }

        public Domain.QueryView FindEntityDefaultView(string entityName)
        {
            List<Domain.QueryView> views = _cacheService.GetVersionItems(entityName, () =>
             {
                 return this.QueryAuthorized(n => n.Where(f => f.EntityName == entityName));
             });
            Domain.QueryView defaultView = null;
            if (views.NotEmpty())
            {
                defaultView = views.FirstOrDefault(n => n.IsDefault == true);
            }
            if (defaultView != null)
            {
                WrapLocalizedLabel(defaultView);
            }
            return defaultView;
        }

        public Domain.QueryView Find(Expression<Func<Domain.QueryView, bool>> predicate)
        {
            var data = _queryViewRepository.Find(predicate);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public List<Domain.QueryView> FindByEntityId(Guid entityId)
        {
            List<Domain.QueryView> views = _cacheService.GetVersionItems(entityId.ToString(), () =>
             {
                 return this.QueryAuthorized(n => n.Where(f => f.EntityId == entityId));
             });
            if (views != null)
            {
                WrapLocalizedLabel(views);
            }
            return views;
        }

        public List<Domain.QueryView> FindByEntityName(string entityName)
        {
            List<Domain.QueryView> views = _cacheService.GetVersionItems(entityName, () =>
             {
                 return this.QueryAuthorized(n => n.Where(f => f.EntityName == entityName));
             });
            if (views != null)
            {
                WrapLocalizedLabel(views);
            }
            return views;
        }

        public List<Domain.QueryView> Query(Func<QueryDescriptor<Domain.QueryView>, QueryDescriptor<Domain.QueryView>> container)
        {
            QueryDescriptor<Domain.QueryView> q = container(QueryDescriptorBuilder.Build<Domain.QueryView>());
            var datas = _queryViewRepository.Query(q);

            WrapLocalizedLabel(datas);
            return datas;
        }

        public PagedList<Domain.QueryView> QueryPaged(Func<QueryDescriptor<Domain.QueryView>, QueryDescriptor<Domain.QueryView>> container)
        {
            QueryDescriptor<Domain.QueryView> q = container(QueryDescriptorBuilder.Build<Domain.QueryView>());
            var datas = _queryViewRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public PagedList<Domain.QueryView> QueryPaged(Func<QueryDescriptor<Domain.QueryView>, QueryDescriptor<Domain.QueryView>> container, Guid solutionId, bool existInSolution)
        {
            QueryDescriptor<Domain.QueryView> q = container(QueryDescriptorBuilder.Build<Domain.QueryView>());
            var datas = _queryViewRepository.QueryPaged(q, ModuleCollection.GetIdentity(QueryViewDefaults.ModuleName), solutionId, existInSolution);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<Domain.QueryView> QueryAuthorized(Func<QueryDescriptor<Domain.QueryView>, QueryDescriptor<Domain.QueryView>> container)
        {
            QueryDescriptor<Domain.QueryView> q = container(QueryDescriptorBuilder.Build<Domain.QueryView>());
            var datas = _queryViewRepository.Query(q).ToList();
            if (!_currentUser.IsSuperAdmin && datas.NotEmpty())
            {
                var authIds = datas.Where(x => x.AuthorizationEnabled).Select(x => x.QueryViewId).ToArray();
                if (authIds.NotEmpty())
                {
                    var authorizedItems = _roleObjectAccessService.Authorized(QueryViewDefaults.ModuleName, authIds);
                    datas.RemoveAll(x => x.AuthorizationEnabled && !authorizedItems.Contains(x.QueryViewId));
                }
            }

            WrapLocalizedLabel(datas);
            return datas;
        }

        public List<Domain.QueryView> FindAll()
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

        private List<Domain.QueryView> PreCacheAll()
        {
            return _queryViewRepository.FindAll()?.ToList();
        }

        #region dependency

        public DependentDescriptor GetDependent(Guid dependentId)
        {
            var result = FindById(dependentId);
            return result != null ? new DependentDescriptor() { Name = result.Name } : null;
        }

        public int ComponentType => ModuleCollection.GetIdentity(QueryViewDefaults.ModuleName);

        #endregion dependency

        private void WrapLocalizedLabel(IEnumerable<Domain.QueryView> datas)
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