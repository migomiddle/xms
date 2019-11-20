using System;
using Xms.Business.DuplicateValidator.Domain;
using Xms.Core.Context;
using Xms.Core.Data;

namespace Xms.Business.DuplicateValidator.Data
{
    public interface IDuplicateRuleRepository : IRepository<DuplicateRule>
    {
        PagedList<Domain.DuplicateRule> QueryPaged(QueryDescriptor<Domain.DuplicateRule> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}