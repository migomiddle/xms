using Xms.Business.DataAnalyse.Domain;

namespace Xms.Business.DataAnalyse.Visualization
{
    public interface IChartCreater
    {
        bool Create(Chart entity);
    }
}