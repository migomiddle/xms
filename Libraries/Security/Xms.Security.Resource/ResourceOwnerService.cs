using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Security.Domain;
using Xms.Security.Resource.Data;

namespace Xms.Security.Resource
{
    /// <summary>
    /// 权限资源类型服务
    /// </summary>
    public class ResourceOwnerService : IResourceOwnerService
    {
        private readonly IResourceOwnerRepository _resourceOwnerRepository;

        public ResourceOwnerService(IResourceOwnerRepository resourceOwnerRepository)
        {
            _resourceOwnerRepository = resourceOwnerRepository;
        }

        public bool Create(Domain.ResourceOwner entity)
        {
            return _resourceOwnerRepository.Create(entity);
        }

        public bool Update(Domain.ResourceOwner entity)
        {
            return _resourceOwnerRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<Domain.ResourceOwner>, UpdateContext<Domain.ResourceOwner>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<Domain.ResourceOwner>());
            return _resourceOwnerRepository.Update(ctx);
        }

        public Domain.ResourceOwner FindById(Guid id)
        {
            return _resourceOwnerRepository.FindById(id);
        }

        public ResourceOwner FindByName(string name)
        {
            return _resourceOwnerRepository.Find(x => x.ModuleName == name);
        }

        public List<Domain.ResourceOwner> FindAll()
        {
            return _resourceOwnerRepository.FindAll()?.ToList();
        }

        public bool DeleteById(Guid id)
        {
            return _resourceOwnerRepository.DeleteById(id);
        }

        public bool DeleteById(IEnumerable<Guid> ids)
        {
            return _resourceOwnerRepository.DeleteMany(ids);
        }

        public PagedList<Domain.ResourceOwner> QueryPaged(Func<QueryDescriptor<Domain.ResourceOwner>, QueryDescriptor<Domain.ResourceOwner>> container)
        {
            QueryDescriptor<Domain.ResourceOwner> q = container(QueryDescriptorBuilder.Build<Domain.ResourceOwner>());

            return _resourceOwnerRepository.QueryPaged(q);
        }

        public List<Domain.ResourceOwner> Query(Func<QueryDescriptor<Domain.ResourceOwner>, QueryDescriptor<Domain.ResourceOwner>> container)
        {
            QueryDescriptor<Domain.ResourceOwner> q = container(QueryDescriptorBuilder.Build<Domain.ResourceOwner>());
            return _resourceOwnerRepository.Query(q)?.ToList();
        }
    }
}