using System;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.DataMapping.Domain;

namespace Xms.DataMapping.Data
{
    public interface IEntityMapRepository : IRepository<EntityMap>
    {
        PagedList<EntityMap> QueryPaged(QueryDescriptor<EntityMap> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}