using System;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Abstractions;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Schema.Data;

namespace Xms.Schema.OptionSet
{
    /// <summary>
    /// 选项集更新服务
    /// </summary>
    public class OptionSetUpdater : IOptionSetUpdater
    {
        private readonly IOptionSetRepository _optionSetRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly Caching.CacheManager<Domain.OptionSet> _cacheService;
        private readonly IAppContext _appContext;

        public OptionSetUpdater(IAppContext appContext
            , IOptionSetRepository optionSetRepository
            , ILocalizedLabelService localizedLabelService)
        {
            _appContext = appContext;
            _optionSetRepository = optionSetRepository;
            _localizedLabelService = localizedLabelService;
            _cacheService = new Caching.CacheManager<Domain.OptionSet>(_appContext.OrganizationUniqueName + ":optionsets", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Update(Domain.OptionSet entity)
        {
            bool result = false;
            using (UnitOfWork.Build(_optionSetRepository.DbContext))
            {
                result = _optionSetRepository.Update(entity);
                //localization
                _localizedLabelService.Update(entity.Name.IfEmpty(""), "LocalizedName", entity.OptionSetId, _appContext.BaseLanguage);
                _localizedLabelService.Update(entity.Description.IfEmpty(""), "Description", entity.OptionSetId, _appContext.BaseLanguage);

                //存在明细项，丢弃缓存，
                _cacheService.RemoveEntity(entity);
            }
            return result;
        }

        public bool Update(Func<UpdateContext<Domain.OptionSet>, UpdateContext<Domain.OptionSet>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<Domain.OptionSet>());
            var flag = _optionSetRepository.Update(ctx);
            if (flag)
            {
                _cacheService.Remove();
            }
            return flag;
        }
    }
}