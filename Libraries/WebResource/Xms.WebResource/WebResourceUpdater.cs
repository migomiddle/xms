using System;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.WebResource.Data;

namespace Xms.WebResource
{
    /// <summary>
    /// web资源更新服务
    /// </summary>
    public class WebResourceUpdater : IWebResourceUpdater
    {
        private readonly IWebResourceRepository _webResourceRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IAppContext _appContext;
        private readonly Caching.CacheManager<Domain.WebResource> _cacheService;

        public WebResourceUpdater(IAppContext appContext
            , IWebResourceRepository webResourceRepository
            , ILocalizedLabelService localizedLabelService)
        {
            _appContext = appContext;
            _webResourceRepository = webResourceRepository;
            _localizedLabelService = localizedLabelService;
            _cacheService = new Caching.CacheManager<Domain.WebResource>(_appContext.OrganizationUniqueName + "webresource", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Update(Domain.WebResource entity)
        {
            var flag = _webResourceRepository.Update(entity);
            if (flag)
            {
                //localization
                _localizedLabelService.Update(entity.Name.IfEmpty(""), "LocalizedName", entity.WebResourceId, _appContext.BaseLanguage);
                _localizedLabelService.Update(entity.Description.IfEmpty(""), "Description", entity.WebResourceId, _appContext.BaseLanguage);
                _cacheService.SetEntity(entity);
            }
            return flag;
        }

        public bool Update(Func<UpdateContext<Domain.WebResource>, UpdateContext<Domain.WebResource>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<Domain.WebResource>());
            return _webResourceRepository.Update(ctx);
        }
    }
}