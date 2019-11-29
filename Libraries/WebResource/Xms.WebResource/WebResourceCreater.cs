using Xms.Context;
using Xms.Data.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Solution;
using Xms.Solution.Abstractions;
using Xms.WebResource.Abstractions;
using Xms.WebResource.Data;

namespace Xms.WebResource
{
    /// <summary>
    /// web资源创建服务
    /// </summary>
    public class WebResourceCreater : IWebResourceCreater
    {
        private readonly IWebResourceRepository _webResourceRepository;
        private readonly ILocalizedLabelBatchBuilder _localizedLabelService;
        private readonly ISolutionComponentService _solutionComponentService;
        private readonly IAppContext _appContext;
        private readonly Caching.CacheManager<Domain.WebResource> _cacheService;

        public WebResourceCreater(IAppContext appContext, IWebResourceRepository webResourceRepository
            , ILocalizedLabelBatchBuilder localizedLabelService
            , ISolutionComponentService solutionComponentService)
        {
            _webResourceRepository = webResourceRepository;
            _localizedLabelService = localizedLabelService;
            _solutionComponentService = solutionComponentService;
            _appContext = appContext;
            _cacheService = new Caching.CacheManager<Domain.WebResource>(_appContext.OrganizationUniqueName + "webresource", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Create(Domain.WebResource entity)
        {
            var solutionid = entity.SolutionId;//当前解决方案
            entity.SolutionId = SolutionDefaults.DefaultSolutionId;//组件属于默认解决方案
            var result = true;
            using (UnitOfWork.Build(_webResourceRepository.DbContext))
            {
                result = _webResourceRepository.Create(entity);
                //solution component
                _solutionComponentService.Create(solutionid, entity.WebResourceId, WebResourceDefaults.ModuleName);
                //本地化标签
                _localizedLabelService.Append(entity.SolutionId, entity.Name.IfEmpty(""), WebResourceDefaults.ModuleName, "LocalizedName", entity.WebResourceId)
                .Append(entity.SolutionId, entity.Description.IfEmpty(""), WebResourceDefaults.ModuleName, "Description", entity.WebResourceId)
                .Save();
                _cacheService.SetEntity(entity);
            }
            return result;
        }
    }
}