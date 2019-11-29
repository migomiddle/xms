using System;
using System.Collections.Generic;
using Xms.Business.DataAnalyse.Data;
using Xms.Business.DataAnalyse.Domain;
using Xms.Context;
using Xms.Core;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Localization;

namespace Xms.Business.DataAnalyse.Visualization
{
    /// <summary>
    /// 图表更新服务
    /// </summary>
    public class ChartUpdater : IChartUpdater
    {
        private readonly IChartRepository _chartRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IAppContext _appContext;
        private readonly IChartDependency _dependencyService;

        public ChartUpdater(IAppContext appContext
            , IChartRepository chartRepository
            , ILocalizedLabelService localizedLabelService
            , IChartDependency dependencyService)
        {
            _appContext = appContext;
            _chartRepository = chartRepository;
            _localizedLabelService = localizedLabelService;
            _dependencyService = dependencyService;
        }

        public bool Update(Chart entity)
        {
            entity.ModifiedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
            entity.ModifiedOn = DateTime.Now;
            var result = _chartRepository.Update(entity);
            if (result)
            {
                //依赖
                _dependencyService.Update(entity);
                //localization
                _localizedLabelService.Update(entity.Name.IfEmpty(""), "LocalizedName", entity.ChartId, _appContext.BaseLanguage);
                _localizedLabelService.Update(entity.Description.IfEmpty(""), "Description", entity.ChartId, _appContext.BaseLanguage);
            }
            return result;
        }

        public bool UpdateState(IEnumerable<Guid> ids, bool isEnabled)
        {
            var context = UpdateContextBuilder.Build<Chart>();
            context.Set(f => f.StateCode, isEnabled ? RecordState.Enabled : RecordState.Disabled)
                .Set(f => f.ModifiedOn, DateTime.Now).Set(f => f.ModifiedBy, _appContext.GetFeature<ICurrentUser>().SystemUserId);
            context.Where(f => f.ChartId.In(ids));
            return _chartRepository.Update(context);
        }
    }
}