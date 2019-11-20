using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Context;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Module.Core;
using Xms.Solution.Abstractions;
using Xms.Solution.Data;
using Xms.Solution.Domain;

namespace Xms.Solution
{
    /// <summary>
    /// 解决方案组件服务
    /// </summary>
    public class SolutionComponentService : ISolutionComponentService
    {
        private readonly ISolutionComponentRepository _solutionComponentRepository;
        private readonly IAppContext _appContext;

        public SolutionComponentService(IAppContext appContext
            , ISolutionComponentRepository solutionComponentRepository)
        {
            _appContext = appContext;
            _solutionComponentRepository = solutionComponentRepository;
        }

        public bool Create(SolutionComponent entity)
        {
            //添加到默认解决方案
            if (entity.SolutionId != SolutionDefaults.DefaultSolutionId)
            {
                var defaultSolutionComponent = new SolutionComponent();
                defaultSolutionComponent.ComponentType = entity.ComponentType;
                defaultSolutionComponent.CreatedBy = entity.CreatedBy;
                defaultSolutionComponent.ObjectId = entity.ObjectId;
                defaultSolutionComponent.SolutionId = SolutionDefaults.DefaultSolutionId;

                _solutionComponentRepository.Create(defaultSolutionComponent);
            }
            return _solutionComponentRepository.Create(entity);
        }

        public bool Create(Guid solutionId, Guid objectId, int componentType)
        {
            var entity = new SolutionComponent();
            entity.ComponentType = componentType;
            entity.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
            entity.ObjectId = objectId;
            entity.SolutionId = solutionId;
            return Create(entity);
        }

        public bool Create(Guid solutionId, Guid objectId, string componentName)
        {
            return this.Create(solutionId, objectId, ModuleCollection.GetIdentity(componentName));
        }

        public bool CreateMany(List<SolutionComponent> entities)
        {
            return _solutionComponentRepository.CreateMany(entities);
        }

        public bool Update(SolutionComponent entity)
        {
            return _solutionComponentRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<SolutionComponent>, UpdateContext<SolutionComponent>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<SolutionComponent>());
            return _solutionComponentRepository.Update(ctx);
        }

        public SolutionComponent FindById(Guid id)
        {
            return _solutionComponentRepository.FindById(id);
        }

        public SolutionComponent Find(Expression<Func<SolutionComponent, bool>> predicate)
        {
            return _solutionComponentRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            return _solutionComponentRepository.DeleteById(id);
        }

        public bool DeleteById(IEnumerable<Guid> ids)
        {
            return _solutionComponentRepository.DeleteMany(ids);
        }

        public bool DeleteObject(Guid solutionid, Guid objectid, int componentType)
        {
            return _solutionComponentRepository.DeleteMany(n => n.SolutionId == solutionid && n.ObjectId == objectid && n.ComponentType == componentType);
        }

        public bool DeleteObject(Guid solutionid, Guid objectid, string componentName)
        {
            return this.DeleteObject(solutionid, objectid, ModuleCollection.GetIdentity(componentName));
        }

        public bool DeleteObject(Guid solutionid, int componentType, params Guid[] objectid)
        {
            if (objectid.IsEmpty())
            {
                return false;
            }
            return _solutionComponentRepository.DeleteMany(x => x.SolutionId == solutionid && x.ComponentType == componentType && x.ObjectId.In(objectid));
        }

        public bool DeleteObject(Guid solutionid, string componentName, params Guid[] objectid)
        {
            return this.DeleteObject(solutionid, ModuleCollection.GetIdentity(componentName), objectid);
        }

        public PagedList<SolutionComponent> QueryPaged(Func<QueryDescriptor<SolutionComponent>, QueryDescriptor<SolutionComponent>> container)
        {
            QueryDescriptor<SolutionComponent> q = container(QueryDescriptorBuilder.Build<SolutionComponent>());

            return _solutionComponentRepository.QueryPaged(q);
        }

        public PagedList<SolutionComponent> QueryPaged(int page, int pageSize, Guid solutionId, string componentTypeName)
        {
            var identity = ModuleCollection.GetIdentity(componentTypeName);
            var q = QueryDescriptorBuilder.Build<SolutionComponent>();
            q.Where(x => x.SolutionId == solutionId && x.ComponentType == identity);
            q.Page(page, pageSize);
            return _solutionComponentRepository.QueryPaged(q);
        }

        public List<SolutionComponent> Query(Func<QueryDescriptor<SolutionComponent>, QueryDescriptor<SolutionComponent>> container)
        {
            QueryDescriptor<SolutionComponent> q = container(QueryDescriptorBuilder.Build<SolutionComponent>());

            return _solutionComponentRepository.Query(q)?.ToList();
        }

        public List<SolutionComponent> Query(Guid solutionId, string componentTypeName)
        {
            var identity = ModuleCollection.GetIdentity(componentTypeName);
            return _solutionComponentRepository.Query(x => x.SolutionId == solutionId && x.ComponentType == identity)?.ToList();
        }
    }
}