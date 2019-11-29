using Xms.Business.DataAnalyse.Domain;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Business.DataAnalyse.Visualization
{
    public interface IChartBuilder
    {
        ChartContext Build(QueryView.Domain.QueryView queryView, Chart chartEntity, FilterExpression filter = null, string drillGroup = "");
    }
}