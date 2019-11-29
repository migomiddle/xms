using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.DataMapping.Domain;

namespace Xms.DataMapping
{
    public interface IAttributeMapFinder
    {
        AttributeMap Find(Expression<Func<AttributeMap, bool>> predicate);

        AttributeMap FindById(Guid id);

        List<AttributeMap> Query(Func<QueryDescriptor<AttributeMap>, QueryDescriptor<AttributeMap>> container);

        PagedList<AttributeMap> QueryPaged(Func<QueryDescriptor<AttributeMap>, QueryDescriptor<AttributeMap>> container);
    }
}