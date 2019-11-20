using System;
using Xms.Core.Context;
using Xms.Core.Data;

namespace Xms.WebResource.Data
{
    public interface IWebResourceRepository : IRepository<Domain.WebResource>
    {
        PagedList<Domain.WebResource> QueryPaged(QueryDescriptor<Domain.WebResource> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}