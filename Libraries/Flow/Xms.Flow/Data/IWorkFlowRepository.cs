using System;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Flow.Domain;

namespace Xms.Flow.Data
{
    public interface IWorkFlowRepository : IRepository<WorkFlow>
    {
        PagedList<WorkFlow> QueryPaged(QueryDescriptor<WorkFlow> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}