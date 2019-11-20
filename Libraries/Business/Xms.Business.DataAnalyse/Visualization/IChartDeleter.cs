using System;

namespace Xms.Business.DataAnalyse.Visualization
{
    public interface IChartDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}