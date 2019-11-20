using System;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Abstractions;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Schema.Data;

namespace Xms.Schema.StringMap
{
    /// <summary>
    /// 字段选项更新服务
    /// </summary>
    public class StringMapUpdater : IStringMapUpdater
    {
        private readonly IStringMapRepository _stringMapRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IAppContext _appContext;

        public StringMapUpdater(IAppContext appContext
            , IStringMapRepository stringMapRepository
            , ILocalizedLabelService localizedLabelService)
        {
            _appContext = appContext;
            _stringMapRepository = stringMapRepository;
            _localizedLabelService = localizedLabelService;
        }

        public bool Update(Domain.StringMap entity)
        {
            var result = false;
            using (UnitOfWork.Build(_stringMapRepository.DbContext))
            {
                result = _stringMapRepository.Update(entity);
                //localization
                _localizedLabelService.Update(entity.Name.IfEmpty(""), "LocalizedName", entity.StringMapId, this._appContext.BaseLanguage);
            }
            return result;
        }

        public bool Update(Func<UpdateContext<Domain.StringMap>, UpdateContext<Domain.StringMap>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<Domain.StringMap>());
            return _stringMapRepository.Update(ctx);
        }
    }
}