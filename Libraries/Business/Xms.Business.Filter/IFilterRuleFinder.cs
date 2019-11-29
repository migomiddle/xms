using System;
using System.Collections.Generic;
using Xms.Business.Filter.Domain;
using Xms.Core;
using Xms.Core.Context;

namespace Xms.Business.Filter
{
    public interface IFilterRuleFinder
    {
        FilterRule FindById(Guid id);

        List<FilterRule> FindAll();

        List<FilterRule> Query(Func<QueryDescriptor<FilterRule>, QueryDescriptor<FilterRule>> container);

        List<FilterRule> QueryByEntityId(Guid entityid, string eventName, RecordState? recordState);

        PagedList<FilterRule> QueryPaged(Func<QueryDescriptor<FilterRule>, QueryDescriptor<FilterRule>> container);

        PagedList<FilterRule> QueryPaged(Func<QueryDescriptor<FilterRule>, QueryDescriptor<FilterRule>> container, Guid solutionId, bool existInSolution);
    }
}