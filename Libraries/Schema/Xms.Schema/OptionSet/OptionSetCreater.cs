using Xms.Context;
using Xms.Data.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Schema.Abstractions;
using Xms.Schema.Data;
using Xms.Solution;
using Xms.Solution.Abstractions;

namespace Xms.Schema.OptionSet
{
    /// <summary>
    /// 选项集创建服务
    /// </summary>
    public class OptionSetCreater : IOptionSetCreater
    {
        private readonly IOptionSetRepository _optionSetRepository;
        private readonly ILocalizedLabelBatchBuilder _localizedLabelService;
        private readonly Caching.CacheManager<Domain.OptionSet> _cacheService;
        private readonly ISolutionComponentService _solutionComponentService;
        private readonly IOptionSetDetailCreater _optionSetDetailCreater;
        private readonly IAppContext _appContext;

        public OptionSetCreater(IAppContext appContext
            , IOptionSetRepository optionSetRepository
            , ISolutionComponentService solutionComponentService
            , IOptionSetDetailCreater optionSetDetailCreater
            , ILocalizedLabelBatchBuilder localizedLabelService)
        {
            _appContext = appContext;
            _optionSetRepository = optionSetRepository;
            _localizedLabelService = localizedLabelService;
            _solutionComponentService = solutionComponentService;
            _optionSetDetailCreater = optionSetDetailCreater;
            _cacheService = new Caching.CacheManager<Domain.OptionSet>(_appContext.OrganizationUniqueName + ":optionsets", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Create(Domain.OptionSet entity)
        {
            Guard.NotNullOrEmpty(entity.Items, "items");
            entity.SolutionId = SolutionDefaults.DefaultSolutionId;//组件属于默认解决方案
            entity.OrganizationId = _appContext.OrganizationId;
            bool result = true;
            using (UnitOfWork.Build(_optionSetRepository.DbContext))
            {
                result = _optionSetRepository.Create(entity);
                //solution component
                if (entity.IsPublic)
                {
                    result = _solutionComponentService.Create(entity.SolutionId, entity.OptionSetId, OptionSetDefaults.ModuleName);
                }
                //存在明细项，不缓存，
                //_cacheService.SetEntity(entity);
                //details
                _optionSetDetailCreater.CreateMany(entity.Items);

                //本地化标签
                _localizedLabelService.Append(entity.SolutionId, entity.Name.IfEmpty(""), OptionSetDefaults.ModuleName, "LocalizedName", entity.OptionSetId)
                .Append(entity.SolutionId, entity.Description.IfEmpty(""), OptionSetDefaults.ModuleName, "Description", entity.OptionSetId)
                .Save();
            }
            return result;
        }
    }
}