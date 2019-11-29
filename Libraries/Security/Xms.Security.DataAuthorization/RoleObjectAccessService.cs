using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Identity;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Module.Core;
using Xms.Security.DataAuthorization.Data;
using Xms.Security.Domain;
using Xms.Security.Resource;

namespace Xms.Security.DataAuthorization
{
    /// <summary>
    /// 角色数据授权服务
    /// </summary>
    public class RoleObjectAccessService : IRoleObjectAccessService, ICascadeDelete<Domain.Role>
    {
        private readonly IRoleObjectAccessRepository _roleObjectAccessRepository;
        private readonly IResourceOwnerService _resourceOwnerService;
        private readonly IAppContext _appContext;

        public RoleObjectAccessService(IAppContext appContext
            , IRoleObjectAccessRepository roleObjectAccessRepository
            , IResourceOwnerService resourceOwnerService
            )
        {
            _appContext = appContext;
            _roleObjectAccessRepository = roleObjectAccessRepository;
            _resourceOwnerService = resourceOwnerService;
        }

        public bool Create(RoleObjectAccess entity)
        {
            return _roleObjectAccessRepository.Create(entity);
        }

        public bool CreateMany(IEnumerable<RoleObjectAccess> entities)
        {
            return _roleObjectAccessRepository.CreateMany(entities);
        }

        public bool CreateMany(string objectTypeName, Guid objectId, params Guid[] roleId)
        {
            Guard.NotEmpty(roleId, nameof(roleId));
            List<RoleObjectAccess> accessList = new List<RoleObjectAccess>();
            foreach (var rid in roleId)
            {
                var ar = new RoleObjectAccess
                {
                    ObjectId = objectId,
                    ObjectTypeCode = ModuleCollection.GetIdentity(objectTypeName),
                    RoleId = rid
                };
                accessList.Add(ar);
            }
            return _roleObjectAccessRepository.CreateMany(accessList);
        }

        public bool Update(RoleObjectAccess entity)
        {
            return _roleObjectAccessRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<RoleObjectAccess>, UpdateContext<RoleObjectAccess>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<RoleObjectAccess>());
            return _roleObjectAccessRepository.Update(ctx);
        }

        public RoleObjectAccess FindById(Guid id)
        {
            return _roleObjectAccessRepository.FindById(id);
        }

        public RoleObjectAccess Find(Expression<Func<RoleObjectAccess, bool>> predicate)
        {
            return _roleObjectAccessRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            return _roleObjectAccessRepository.DeleteById(id);
        }

        public bool DeleteById(IEnumerable<Guid> ids)
        {
            return _roleObjectAccessRepository.DeleteMany(ids);
        }

        public bool DeleteByObjectId(int objectTypeCode, params Guid[] objectId)
        {
            return _roleObjectAccessRepository.DeleteMany(x => x.ObjectTypeCode == objectTypeCode && x.ObjectId.In(objectId));
        }

        public bool DeleteByObjectId(string objectTypeName, params Guid[] objectId)
        {
            return this.DeleteByObjectId(ModuleCollection.GetIdentity(objectTypeName), objectId);
        }

        public bool DeleteByObjectId(Guid objectId, int objectTypeCode)
        {
            return _roleObjectAccessRepository.DeleteMany(x => x.ObjectId == objectId && x.ObjectTypeCode == objectTypeCode);
        }

        public bool DeleteByObjectId(Guid objectId, string objectTypeName)
        {
            return this.DeleteByObjectId(objectId, ModuleCollection.GetIdentity(objectTypeName));
        }

        public bool DeleteByRole(Guid roleId, int objectTypeCode)
        {
            return _roleObjectAccessRepository.DeleteMany(x => x.RoleId == roleId && x.ObjectTypeCode == objectTypeCode);
        }

        public bool DeleteByRole(Guid roleId, string objectTypeName)
        {
            return this.DeleteByRole(roleId, ModuleCollection.GetIdentity(objectTypeName));
        }

        public PagedList<RoleObjectAccess> QueryPaged(Func<QueryDescriptor<RoleObjectAccess>, QueryDescriptor<RoleObjectAccess>> container)
        {
            QueryDescriptor<RoleObjectAccess> q = container(QueryDescriptorBuilder.Build<RoleObjectAccess>());

            return _roleObjectAccessRepository.QueryPaged(q);
        }

        public List<RoleObjectAccess> Query(Func<QueryDescriptor<RoleObjectAccess>, QueryDescriptor<RoleObjectAccess>> container)
        {
            QueryDescriptor<RoleObjectAccess> q = container(QueryDescriptorBuilder.Build<RoleObjectAccess>());
            return _roleObjectAccessRepository.Query(q)?.ToList();
        }

        public List<RoleObjectAccess> Query(Guid objectId, string objectTypeName)
        {
            return _roleObjectAccessRepository.Query(x => x.ObjectId == objectId && x.ObjectTypeCode == ModuleCollection.GetIdentity(objectTypeName))?.ToList();
        }

        public List<RoleObjectAccess> QueryRolePermissions(Guid roleId, string objectTypeName)
        {
            return _roleObjectAccessRepository.Query(x => x.RoleId == roleId && x.ObjectTypeCode == ModuleCollection.GetIdentity(objectTypeName))?.ToList();
        }

        public bool Exists(Guid objectId, int objectTypeCode, params Guid[] roleId)
        {
            var result = this._roleObjectAccessRepository.Find(n => n.ObjectId == objectId && n.ObjectTypeCode == objectTypeCode && n.RoleId.In(roleId));
            return result != null;
        }

        public bool Exists(Guid objectId, string objectTypeName, params Guid[] roleId)
        {
            return this.Exists(objectId, ModuleCollection.GetIdentity(objectTypeName), roleId);
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的安全角色</param>
        public void CascadeDelete(params Domain.Role[] parent)
        {
            if (parent.NotEmpty())
            {
                var ids = parent.Select(x => x.RoleId);
                _roleObjectAccessRepository.DeleteMany(x => x.RoleId.In(ids));
            }
        }

        public List<Guid> Authorized(string objectTypeName, params Guid[] objectId)
        {
            List<Guid> result = new List<Guid>();
            if (objectId.IsEmpty())
            {
                return result;
            }
            int objectTypeCode = ModuleCollection.GetIdentity(objectTypeName);
            var resourceOwner = _resourceOwnerService.FindByName(objectTypeName);
            if (resourceOwner == null || resourceOwner.StateCode == Core.RecordState.Disabled)
            {
                return result;
            }
            var userInRole = _appContext.GetFeature<ICurrentUser>().Roles.Select(f => f.RoleId).ToArray();
            if (userInRole.NotEmpty())
            {
                var roaList = _roleObjectAccessRepository.Query(x => x.RoleId.In(userInRole) && x.ObjectTypeCode == objectTypeCode && x.ObjectId.In(objectId));
                result = objectId.ToList();
                result.RemoveAll(x => !roaList.Exists(r => r.ObjectId == x));
                return result;
            }
            return result;
        }
    }
}