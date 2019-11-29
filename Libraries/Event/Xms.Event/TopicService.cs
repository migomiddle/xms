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
    /// 事件主题服务
    /// </summary>
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _topicRepository;

        public TopicService(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        public bool Create(Topic entity)
        {
            return _topicRepository.Create(entity);
        }

        public bool CreateMany(List<Topic> entities)
        {
            return _topicRepository.CreateMany(entities);
        }

        public bool Update(Topic entity)
        {
            return _topicRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<Topic>, UpdateContext<Topic>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<Topic>());
            return _topicRepository.Update(ctx);
        }

        public Topic FindById(Guid id)
        {
            return _topicRepository.FindById(id);
        }

        public Topic Find(Expression<Func<Topic, bool>> predicate)
        {
            return _topicRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            return _topicRepository.DeleteById(id);
        }

        public bool DeleteById(List<Guid> ids)
        {
            return _topicRepository.DeleteMany(ids);
        }

        public PagedList<Topic> QueryPaged(Func<QueryDescriptor<Topic>, QueryDescriptor<Topic>> container)
        {
            QueryDescriptor<Topic> q = container(QueryDescriptorBuilder.Build<Topic>());

            return _topicRepository.QueryPaged(q);
        }

        public List<Topic> Query(Func<QueryDescriptor<Topic>, QueryDescriptor<Topic>> container)
        {
            QueryDescriptor<Topic> q = container(QueryDescriptorBuilder.Build<Topic>());

            return _topicRepository.Query(q)?.ToList();
        }
    }
}