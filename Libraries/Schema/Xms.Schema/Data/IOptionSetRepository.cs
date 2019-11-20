using System;
using Xms.Core.Context;
using Xms.Core.Data;

namespace Xms.Schema.Data
{
    public interface IOptionSetRepository : IRepository<Domain.OptionSet>
    {
        PagedList<Domain.OptionSet> QueryPaged(QueryDescriptor<Domain.OptionSet> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}