using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;

namespace Xms.Schema.StringMap
{
    public interface IStringMapFinder
    {
        Domain.StringMap Find(Expression<Func<Domain.StringMap, bool>> predicate);

        Domain.StringMap FindById(Guid id);

        List<Domain.StringMap> FindByAttributeId(Guid attributeId);

        string GetOptionName(Guid attributeId, int value);

        List<Domain.StringMap> Query(Func<QueryDescriptor<Domain.StringMap>, QueryDescriptor<Domain.StringMap>> container);

        PagedList<Domain.StringMap> QueryPaged(Func<QueryDescriptor<Domain.StringMap>, QueryDescriptor<Domain.StringMap>> container);
    }
}