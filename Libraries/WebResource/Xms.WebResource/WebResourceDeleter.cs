using System;
using Xms.Context;
using Xms.Dependency;
using Xms.Infrastructure;
using Xms.Localization;
using Xms.Solution;
using Xms.WebResource.Abstractions;
using Xms.WebResource.Data;

namespace Xms.WebResource
{
    /// <summary>
    /// web资源删除服务
    /// </summary>
    public class WebResourceDeleter : IWebResourceDeleter
    {
        private readonly IWebResourceRepository _webResourceRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly ISolutionComponentService _solutionComponentService;
        private readonly IDependencyService _dependencyService;
        private readonly IDependencyChecker _dependencyChecker;
        private readonly IAppContext _appContext;
        private readonly Caching.CacheManager<Domain.WebResource> _cacheService;

        public WebResourceDeleter(IAppContext appContext
            , IWebResourceRepository webResourceRepository
            , ILocalizedLabelService localizedLabelService
            , ISolutionComponentService solutionComponentService
            , IDependencyService dependencyService
            , IDependencyChecker dependencyChecker)
        {
            _appContext = appContext;
            _webResourceRepository = webResourceRepository;
            _localizedLabelService = localizedLabelService;
            _solutionComponentService = solutionComponentService;
            _dependencyService = dependencyService;
            _dependencyChecker = dependencyChecker;
            _cacheService = new Caching.CacheManager<Domain.WebResource>(_appContext.OrganizationUniqueName + "webresource", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = true;
            foreach (var item in id)
            {
                var deleted = _webResourceRepository.FindById(item);
                if (deleted == null)
                {
                    return false;
                }
                //检查依赖项
                _dependencyChecker.CheckAndThrow<Domain.WebResource>(WebResourceDefaults.ModuleName, deleted.WebResourceId);
                bool flag = _webResourceRepository.DeleteById(item);
                if (flag)
                {
                    //删除依赖项
                    _dependencyService.DeleteByDependentId(WebResourceDefaults.ModuleName, id);
                    //solution component
                    _solutionComponentService.DeleteObject(deleted.SolutionId, deleted.WebResourceId, WebResourceDefaults.ModuleName);
                    //localization
                    _localizedLabelService.DeleteByObject(item);
                    _cacheService.RemoveEntity(deleted);
                }
            }
            return result;
        }
    }
}