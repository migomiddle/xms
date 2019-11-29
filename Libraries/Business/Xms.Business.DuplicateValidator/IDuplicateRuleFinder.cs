using System;
using System.Collections.Generic;
using Xms.Business.DuplicateValidator.Domain;
using Xms.Core;
using Xms.Core.Context;

namespace Xms.Business.DuplicateValidator
{
    public interface IDuplicateRuleFinder
    {
        DuplicateRule FindById(Guid id);

        List<DuplicateRule> FindAll();

        List<DuplicateRule> Query(Func<QueryDescriptor<DuplicateRule>, QueryDescriptor<DuplicateRule>> container);

        List<DuplicateRule> QueryByEntityId(Guid entityid, RecordState? state);

        PagedList<DuplicateRule> QueryPaged(Func<QueryDescriptor<DuplicateRule>, QueryDescriptor<DuplicateRule>> container);

        PagedList<DuplicateRule> QueryPaged(Func<QueryDescriptor<DuplicateRule>, QueryDescriptor<DuplicateRule>> container, Guid solutionId, bool existInSolution);
    }
}