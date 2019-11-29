using System;
using System.Collections.Generic;
using Xms.Business.DataAnalyse.Domain;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Query;

namespace Xms.Business.DataAnalyse.Data
{
    public interface IChartRepository : IRepository<Chart>
    {
        List<dynamic> GetChartDataSource(ChartDataDescriptor chartData, QueryExpression query, IQueryResolver queryResolver);

        PagedList<Domain.Chart> QueryPaged(QueryDescriptor<Domain.Chart> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}