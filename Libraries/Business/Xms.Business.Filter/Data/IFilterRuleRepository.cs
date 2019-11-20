using System;
using Xms.Business.Filter.Domain;
using Xms.Core.Context;
using Xms.Core.Data;

namespace Xms.Business.Filter.Data
{
    public interface IFilterRuleRepository : IRepository<FilterRule>
    {
        PagedList<FilterRule> QueryPaged(QueryDescriptor<FilterRule> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}