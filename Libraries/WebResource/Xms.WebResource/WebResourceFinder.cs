using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Configuration;
using Xms.Context;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Module.Core;
using Xms.WebResource.Abstractions;
using Xms.WebResource.Data;

namespace Xms.WebResource
{
    /// <summary>
    /// web资源查询服务
    /// </summary>
    public class WebResourceFinder : IWebResourceFinder
    {
        private readonly IWebResourceRepository _webResourceRepository;
        private readonly IAppContext _appContext;
        private readonly Caching.CacheManager<Domain.WebResource> _cacheService;

        public WebResourceFinder(IAppContext appContext, ISettingFinder settingFinder, IWebResourceRepository webResourceRepository)
        {
            _appContext = appContext;
            _webResourceRepository = webResourceRepository;
            _cacheService = new Caching.CacheManager<Domain.WebResource>(_appContext.OrganizationUniqueName + "webresource", _appContext.PlatformSettings.CacheEnabled);
        }

        public static string BuildKey(Domain.WebResource entity)
        {
            return "";
        }

        private List<Domain.WebResource> PreCacheAll()
        {
            return new List<Domain.WebResource>();
        }

        public Domain.WebResource FindById(Guid id, bool IsPublished = true)
        {
            var data = _webResourceRepository.FindById(id);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public List<Domain.WebResource> FindByIds(params Guid[] ids)
        {
            string sIndex = string.Join("/", ids);
            var datas = _cacheService.GetVersionItems(sIndex, () =>
             {
                 return _webResourceRepository.Query(n => n.WebResourceId.In(ids))?.ToList();
             }
            );

            WrapLocalizedLabel(datas);
            return datas;
        }

        public Domain.WebResource Find(Expression<Func<Domain.WebResource, bool>> predicate)
        {
            var data = _webResourceRepository.Find(predicate);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public PagedList<Domain.WebResource> QueryPaged(Func<QueryDescriptor<Domain.WebResource>, QueryDescriptor<Domain.WebResource>> container)
        {
            QueryDescriptor<Domain.WebResource> q = container(QueryDescriptorBuilder.Build<Domain.WebResource>());
            var datas = _webResourceRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public PagedList<Domain.WebResource> QueryPaged(Func<QueryDescriptor<Domain.WebResource>, QueryDescriptor<Domain.WebResource>> container, Guid solutionId, bool existInSolution)
        {
            QueryDescriptor<Domain.WebResource> q = container(QueryDescriptorBuilder.Build<Domain.WebResource>());
            var datas = _webResourceRepository.QueryPaged(q, ModuleCollection.GetIdentity(WebResourceDefaults.ModuleName), solutionId, existInSolution);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<Domain.WebResource> Query(Func<QueryDescriptor<Domain.WebResource>, QueryDescriptor<Domain.WebResource>> container)
        {
            QueryDescriptor<Domain.WebResource> q = container(QueryDescriptorBuilder.Build<Domain.WebResource>());
            var datas = _webResourceRepository.Query(q)?.ToList();

            WrapLocalizedLabel(datas);
            return datas;
        }

        private void WrapLocalizedLabel(IEnumerable<Domain.WebResource> datas)
        {
            /*
            if (datas.NotEmpty())
            {
                var ids = datas.Select(f => f.WebResourceId);
                var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == _appContext.CurrentUser.UserSettings.LanguageId && f.ObjectId.In(ids)));
                foreach (var d in datas)
                {
                    d.Name = _localizedLabelService.GetLabelText(labels, d.WebResourceId, "LocalizedName", d.Name);
                    d.Description = _localizedLabelService.GetLabelText(labels, d.WebResourceId, "Description", d.Description);
                }
            }
            */
        }

        private void WrapLocalizedLabel(Domain.WebResource entity)
        {
            /*
            var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == _appContext.CurrentUser.UserSettings.LanguageId && f.ObjectId == entity.WebResourceId));
            entity.Name = _localizedLabelService.GetLabelText(labels, entity.WebResourceId, "LocalizedName", entity.Name);
            entity.Description = _localizedLabelService.GetLabelText(labels, entity.WebResourceId, "Description", entity.Description);
            */
        }
    }
}