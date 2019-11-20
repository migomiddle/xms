using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Authorization.Abstractions;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Security.DataAuthorization.Data;
using Xms.Security.Domain;

namespace Xms.Security.DataAuthorization
{
    /// <summary>
    /// 数据共享/授权服务
    /// </summary>
    public class PrincipalObjectAccessService : IPrincipalObjectAccessService, ICascadeDelete<Schema.Domain.Entity>
    {
        private readonly IPrincipalObjectAccessRepository _principalObjectAccessRepository;

        public PrincipalObjectAccessService(IPrincipalObjectAccessRepository principalObjectAccessRepository)
        {
            _principalObjectAccessRepository = principalObjectAccessRepository;
        }

        public bool Create(PrincipalObjectAccess entity)
        {
            return _principalObjectAccessRepository.Create(entity);
        }

        public bool CreateMany(IEnumerable<PrincipalObjectAccess> entities)
        {
            return _principalObjectAccessRepository.CreateMany(entities);
        }

        public bool Update(PrincipalObjectAccess entity)
        {
            return _principalObjectAccessRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<PrincipalObjectAccess>, UpdateContext<PrincipalObjectAccess>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<PrincipalObjectAccess>());
            return _principalObjectAccessRepository.Update(ctx);
        }

        public PrincipalObjectAccess FindById(Guid id)
        {
            return _principalObjectAccessRepository.FindById(id);
        }

        public PrincipalObjectAccess Find(Expression<Func<PrincipalObjectAccess, bool>> predicate)
        {
            return _principalObjectAccessRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            return _principalObjectAccessRepository.DeleteById(id);
        }

        public bool DeleteById(IEnumerable<Guid> ids)
        {
            return _principalObjectAccessRepository.DeleteMany(ids);
        }

        public bool DeleteByObjectId(Guid entityId, Guid objectId)
        {
            return _principalObjectAccessRepository.DeleteMany(x => x.EntityId == entityId && x.ObjectId == objectId);
        }

        public PagedList<PrincipalObjectAccess> QueryPaged(Func<QueryDescriptor<PrincipalObjectAccess>, QueryDescriptor<PrincipalObjectAccess>> container)
        {
            QueryDescriptor<PrincipalObjectAccess> q = container(QueryDescriptorBuilder.Build<PrincipalObjectAccess>());

            return _principalObjectAccessRepository.QueryPaged(q);
        }

        public List<PrincipalObjectAccess> Query(Func<QueryDescriptor<PrincipalObjectAccess>, QueryDescriptor<PrincipalObjectAccess>> container)
        {
            QueryDescriptor<PrincipalObjectAccess> q = container(QueryDescriptorBuilder.Build<PrincipalObjectAccess>());

            return _principalObjectAccessRepository.Query(q)?.ToList();
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的实体</param>
        public void CascadeDelete(params Schema.Domain.Entity[] parent)
        {
            if (parent.NotEmpty())
            {
                var ids = parent.Select(x => x.EntityId);
                _principalObjectAccessRepository.DeleteMany(x => x.EntityId.In(ids));
            }
        }
    }
}