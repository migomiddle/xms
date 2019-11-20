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
    /// 选项集项目更新服务
    /// </summary>
    public class OptionSetDetailUpdater : IOptionSetDetailUpdater
    {
        private readonly IOptionSetDetailRepository _optionSetDetailRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IAppContext _appContext;

        public OptionSetDetailUpdater(IAppContext appContext
            , IOptionSetDetailRepository optionSetDetailRepository
            , ILocalizedLabelService localizedLabelService)
        {
            _appContext = appContext;
            _optionSetDetailRepository = optionSetDetailRepository;
            _localizedLabelService = localizedLabelService;
        }

        public bool Update(Domain.OptionSetDetail entity)
        {
            bool result = false;
            using (UnitOfWork.Build(_optionSetDetailRepository.DbContext))
            {
                result = _optionSetDetailRepository.Update(entity);
                //localization
                _localizedLabelService.Update(entity.Name.IfEmpty(""), "LocalizedName", entity.OptionSetDetailId, _appContext.BaseLanguage);
            }
            return result;
        }

        public bool Update(Func<UpdateContext<Domain.OptionSetDetail>, UpdateContext<Domain.OptionSetDetail>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<Domain.OptionSetDetail>());
            return _optionSetDetailRepository.Update(ctx);
        }
    }
}