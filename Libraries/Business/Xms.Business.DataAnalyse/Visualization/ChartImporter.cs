using System;
using System.Collections.Generic;
using Xms.Business.DataAnalyse.Domain;
using Xms.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Business.DataAnalyse.Visualization
{
    /// <summary>
    /// 图表导入服务
    /// </summary>
    [SolutionImportNode("charts")]
    public class ChartImporter : ISolutionComponentImporter<Chart>
    {
        private readonly IChartCreater _chartCreater;
        private readonly IChartUpdater _chartUpdater;
        private readonly IChartFinder _chartFinder;
        private readonly IAppContext _appContext;

        public ChartImporter(IAppContext appContext
            , IChartCreater chartCreater
            , IChartUpdater chartUpdater
            , IChartFinder chartFinder)
        {
            _appContext = appContext;
            _chartCreater = chartCreater;
            _chartUpdater = chartUpdater;
            _chartFinder = chartFinder;
        }

        public bool Import(Guid solutionId, IList<Chart> charts)
        {
            if (charts.NotEmpty())
            {
                foreach (var item in charts)
                {
                    var entity = _chartFinder.FindById(item.ChartId);
                    if (entity != null)
                    {
                        entity.DataConfig = item.DataConfig;
                        entity.Description = item.Description;
                        entity.Name = item.Name;
                        entity.PresentationConfig = item.PresentationConfig;
                        entity.StateCode = item.StateCode;
                        _chartUpdater.Update(entity);
                    }
                    else
                    {
                        item.SolutionId = solutionId;
                        item.ComponentState = 0;
                        item.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        item.OrganizationId = _appContext.OrganizationId;
                        _chartCreater.Create(item);
                    }
                }
            }
            return true;
        }
    }
}