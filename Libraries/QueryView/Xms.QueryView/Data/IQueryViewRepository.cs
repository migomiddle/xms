using System;
using Xms.Core.Context;
using Xms.Core.Data;

namespace Xms.QueryView.Data
{
    public interface IQueryViewRepository : IRepository<Domain.QueryView>
    {
        PagedList<Domain.QueryView> QueryPaged(QueryDescriptor<Domain.QueryView> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}