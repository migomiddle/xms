using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Event.Data;
using Xms.Event.Domain;

namespace Xms.Event
{
    /// <summary>
    /// 事件消息服务
    /// </summary>
    public class EventMessageService : IEventMessageService
    {
        private readonly IEventMessageRepository _eventMessageRepository;

        public EventMessageService(IEventMessageRepository eventMessageRepository)
        {
            _eventMessageRepository = eventMessageRepository;
        }

        public bool Create(EventMessage entity)
        {
            return _eventMessageRepository.Create(entity);
        }

        public bool CreateMany(List<EventMessage> entities)
        {
            return _eventMessageRepository.CreateMany(entities);
        }

        public bool Update(EventMessage entity)
        {
            return _eventMessageRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<EventMessage>, UpdateContext<EventMessage>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<EventMessage>());
            return _eventMessageRepository.Update(ctx);
        }

        public EventMessage FindById(Guid id)
        {
            return _eventMessageRepository.FindById(id);
        }

        public EventMessage Find(Expression<Func<EventMessage, bool>> predicate)
        {
            return _eventMessageRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            return _eventMessageRepository.DeleteById(id);
        }

        public bool DeleteById(List<Guid> ids)
        {
            return _eventMessageRepository.DeleteMany(ids);
        }

        public PagedList<EventMessage> QueryPaged(Func<QueryDescriptor<EventMessage>, QueryDescriptor<EventMessage>> container)
        {
            QueryDescriptor<EventMessage> q = container(QueryDescriptorBuilder.Build<EventMessage>());

            return _eventMessageRepository.QueryPaged(q);
        }

        public List<EventMessage> Query(Func<QueryDescriptor<EventMessage>, QueryDescriptor<EventMessage>> container)
        {
            QueryDescriptor<EventMessage> q = container(QueryDescriptorBuilder.Build<EventMessage>());

            return _eventMessageRepository.Query(q)?.ToList();
        }
    }
}