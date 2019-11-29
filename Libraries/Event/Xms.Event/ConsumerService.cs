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
    /// 事件消费者服务
    /// </summary>
    public class ConsumerService : IConsumerService
    {
        private readonly IConsumerRepository _consumerRepository;

        public ConsumerService(IConsumerRepository consumerRepository)
        {
            _consumerRepository = consumerRepository;
        }

        public bool Create(Consumer entity)
        {
            return _consumerRepository.Create(entity);
        }

        public bool CreateMany(List<Consumer> entities)
        {
            return _consumerRepository.CreateMany(entities);
        }

        public bool Update(Consumer entity)
        {
            return _consumerRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<Consumer>, UpdateContext<Consumer>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<Consumer>());
            return _consumerRepository.Update(ctx);
        }

        public Consumer FindById(Guid id)
        {
            return _consumerRepository.FindById(id);
        }

        public Consumer Find(Expression<Func<Consumer, bool>> predicate)
        {
            return _consumerRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            return _consumerRepository.DeleteById(id);
        }

        public bool DeleteById(List<Guid> ids)
        {
            return _consumerRepository.DeleteMany(ids);
        }

        public PagedList<Consumer> QueryPaged(Func<QueryDescriptor<Consumer>, QueryDescriptor<Consumer>> container)
        {
            QueryDescriptor<Consumer> q = container(QueryDescriptorBuilder.Build<Consumer>());

            return _consumerRepository.QueryPaged(q);
        }

        public List<Consumer> Query(Func<QueryDescriptor<Consumer>, QueryDescriptor<Consumer>> container)
        {
            QueryDescriptor<Consumer> q = container(QueryDescriptorBuilder.Build<Consumer>());

            return _consumerRepository.Query(q)?.ToList();
        }
    }
}