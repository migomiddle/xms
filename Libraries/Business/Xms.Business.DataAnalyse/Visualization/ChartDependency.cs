using System;
using System.Linq;
using Xms.Business.DataAnalyse.Domain;
using Xms.Dependency;
using Xms.QueryView;
using Xms.Schema.Abstractions;

namespace Xms.Business.DataAnalyse.Visualization
{
    /// <summary>
    /// 图表依赖服务
    /// </summary>
    public class ChartDependency : IChartDependency
    {
        private readonly IDependencyService _dependencyService;
        private readonly IChartBuilder _chartBuilder;
        private readonly IQueryViewFinder _queryViewFinder;

        public ChartDependency(IDependencyService dependencyService
            , IQueryViewFinder queryViewFinder
            , IChartBuilder chartBuilder)
        {
            _dependencyService = dependencyService;
            _queryViewFinder = queryViewFinder;
            _chartBuilder = chartBuilder;
        }

        public bool Create(Chart entity)
        {
            bool result = true;
            var chart = _chartBuilder.Build(_queryViewFinder.FindEntityDefaultView(entity.EntityId), entity);
            //依赖于字段
            _dependencyService.Create(ChartDefaults.ModuleName, entity.ChartId, AttributeDefaults.ModuleName, chart.Attributes.Select(x => x.AttributeId).ToArray());
            return result;
        }

        public bool Update(Chart entity)
        {
            bool result = true;
            var chart = _chartBuilder.Build(_queryViewFinder.FindEntityDefaultView(entity.EntityId), entity);
            //依赖于字段
            _dependencyService.Update(ChartDefaults.ModuleName, entity.ChartId, AttributeDefaults.ModuleName, chart.Attributes.Select(x => x.AttributeId).ToArray());
            return result;
        }

        public bool Delete(params Guid[] id)
        {
            //删除依赖项
            return _dependencyService.DeleteByDependentId(ChartDefaults.ModuleName, id);
        }
    }
}