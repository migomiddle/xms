using System;
using System.Collections.Generic;
using Xms.Business.DataAnalyse.Domain;

namespace Xms.Business.DataAnalyse.Visualization
{
    public interface IChartUpdater
    {
        bool Update(Chart entity);

        bool UpdateState(IEnumerable<Guid> ids, bool isEnabled);
    }
}