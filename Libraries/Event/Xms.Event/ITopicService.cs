using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Event.Domain;

namespace Xms.Event
{
    public interface ITopicService
    {
        bool Create(Topic entity);

        bool CreateMany(List<Topic> entities);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        Topic Find(Expression<Func<Topic, bool>> predicate);

        Topic FindById(Guid id);

        List<Topic> Query(Func<QueryDescriptor<Topic>, QueryDescriptor<Topic>> container);

        PagedList<Topic> QueryPaged(Func<QueryDescriptor<Topic>, QueryDescriptor<Topic>> container);

        bool Update(Func<UpdateContext<Topic>, UpdateContext<Topic>> context);

        bool Update(Topic entity);
    }
}