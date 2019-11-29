using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Event.Domain;

namespace Xms.Event
{
    public interface IConsumerService
    {
        bool Create(Consumer entity);

        bool CreateMany(List<Consumer> entities);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        Consumer Find(Expression<Func<Consumer, bool>> predicate);

        Consumer FindById(Guid id);

        List<Consumer> Query(Func<QueryDescriptor<Consumer>, QueryDescriptor<Consumer>> container);

        PagedList<Consumer> QueryPaged(Func<QueryDescriptor<Consumer>, QueryDescriptor<Consumer>> container);

        bool Update(Consumer entity);

        bool Update(Func<UpdateContext<Consumer>, UpdateContext<Consumer>> context);
    }
}