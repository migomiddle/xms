using System;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Plugin.Domain;

namespace Xms.Plugin.Data
{
    public interface IEntityPluginRepository : IRepository<EntityPlugin>
    {
        PagedList<EntityPlugin> QueryPaged(QueryDescriptor<EntityPlugin> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}