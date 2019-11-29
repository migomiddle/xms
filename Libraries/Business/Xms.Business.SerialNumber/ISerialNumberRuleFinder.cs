using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;

namespace Xms.Business.SerialNumber
{
    public interface ISerialNumberRuleFinder
    {
        Domain.SerialNumberRule Find(Expression<Func<Domain.SerialNumberRule, bool>> predicate);

        Domain.SerialNumberRule FindByEntityId(Guid entityid);

        Domain.SerialNumberRule FindById(Guid id);

        List<Domain.SerialNumberRule> FindAll();

        List<Domain.SerialNumberRule> Query(Func<QueryDescriptor<Domain.SerialNumberRule>, QueryDescriptor<Domain.SerialNumberRule>> container);

        PagedList<Domain.SerialNumberRule> QueryPaged(Func<QueryDescriptor<Domain.SerialNumberRule>, QueryDescriptor<Domain.SerialNumberRule>> container);

        PagedList<Domain.SerialNumberRule> QueryPaged(Func<QueryDescriptor<Domain.SerialNumberRule>, QueryDescriptor<Domain.SerialNumberRule>> container, Guid solutionId, bool existInSolution);
    }
}