using System;
using Xms.Core.Context;
using Xms.Core.Data;

namespace Xms.Form.Data
{
    public interface ISystemFormRepository : IRepository<Domain.SystemForm>
    {
        PagedList<Domain.SystemForm> QueryPaged(QueryDescriptor<Domain.SystemForm> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}