using System;
using Xms.Business.DataAnalyse.Domain;

namespace Xms.Business.DataAnalyse.Visualization
{
    public interface IChartDependency
    {
        bool Create(Chart entity);

        bool Delete(params Guid[] id);

        bool Update(Chart entity);
    }
}