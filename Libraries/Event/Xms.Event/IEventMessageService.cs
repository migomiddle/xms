using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Event.Domain;

namespace Xms.Event
{
    public interface IEventMessageService
    {
        bool Create(EventMessage entity);

        bool CreateMany(List<EventMessage> entities);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        EventMessage Find(Expression<Func<EventMessage, bool>> predicate);

        EventMessage FindById(Guid id);

        List<EventMessage> Query(Func<QueryDescriptor<EventMessage>, QueryDescriptor<EventMessage>> container);

        PagedList<EventMessage> QueryPaged(Func<QueryDescriptor<EventMessage>, QueryDescriptor<EventMessage>> container);

        bool Update(EventMessage entity);

        bool Update(Func<UpdateContext<EventMessage>, UpdateContext<EventMessage>> context);
    }
}