using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Security.Abstractions;
using Xms.Security.Domain;
using Xms.Security.Principal.Data;

namespace Xms.Security.Principal
{
    /// <summary>
    /// 用户安全角色服务
    /// </summary>
    public class SystemUserRolesService : ISystemUserRolesService, ICascadeDelete<Role>
    {
        private readonly ISystemUserRolesRepository _systemUserRolesRepository;

        public SystemUserRolesService(ISystemUserRolesRepository systemUserRolesRepository)
        {
            _systemUserRolesRepository = systemUserRolesRepository;
        }

        public bool Create(SystemUserRoles entity)
        {
            return _systemUserRolesRepository.Create(entity);
        }

        public bool CreateMany(List<SystemUserRoles> entities)
        {
            return _systemUserRolesRepository.CreateMany(entities);
        }

        public bool Update(SystemUserRoles entity)
        {
            return _systemUserRolesRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<SystemUserRoles>, UpdateContext<SystemUserRoles>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<SystemUserRoles>());
            return _systemUserRolesRepository.Update(ctx);
        }

        public bool UpdateUserRoles(Guid systemUserId, List<SystemUserRoles> roles)
        {
            var userroles = Query(q => q.Where(f => f.SystemUserId == systemUserId));
            DeleteById(userroles.Select(f => f.SystemUserRoleId).ToList());
            if (roles.NotEmpty())
            {
                return CreateMany(roles);
            }
            return false;
        }

        public SystemUserRoles FindById(Guid id)
        {
            return _systemUserRolesRepository.FindById(id);
        }

        public List<SystemUserRoles> FindByUserId(Guid systemUserId)
        {
            return this.Query(n => n.Where(f => f.SystemUserId == systemUserId));
        }

        public SystemUserRoles Find(Expression<Func<SystemUserRoles, bool>> predicate)
        {
            return _systemUserRolesRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            return _systemUserRolesRepository.DeleteById(id);
        }

        public bool DeleteById(List<Guid> ids)
        {
            return _systemUserRolesRepository.DeleteMany(ids);
        }

        public bool DeleteById(Guid userId, Guid roleid)
        {
            return _systemUserRolesRepository.DeleteMany(x => x.SystemUserId == userId && x.RoleId == roleid);
        }

        public PagedList<SystemUserRoles> QueryPaged(Func<QueryDescriptor<SystemUserRoles>, QueryDescriptor<SystemUserRoles>> container)
        {
            QueryDescriptor<SystemUserRoles> q = container(QueryDescriptorBuilder.Build<SystemUserRoles>());

            return _systemUserRolesRepository.QueryPaged(q);
        }

        public List<SystemUserRoles> Query(Func<QueryDescriptor<SystemUserRoles>, QueryDescriptor<SystemUserRoles>> container)
        {
            QueryDescriptor<SystemUserRoles> q = container(QueryDescriptorBuilder.Build<SystemUserRoles>());

            return _systemUserRolesRepository.Query(q)?.ToList();
        }

        public bool IsAdministrator(Guid systemUserId)
        {
            return this.Find(n => n.SystemUserId == systemUserId && n.RoleName == RoleDefaults.ADMINISTRATOR) != null;
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的安全角色</param>
        public void CascadeDelete(params Role[] parent)
        {
            if (parent.NotEmpty())
            {
                var ids = parent.Select(x => x.RoleId);
                _systemUserRolesRepository.DeleteMany(x => x.RoleId.In(ids));
            }
        }
    }
}