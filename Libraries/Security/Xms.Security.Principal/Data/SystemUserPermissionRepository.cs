using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure.Utility;
using Xms.SiteMap.Domain;

namespace Xms.Security.Principal.Data
{
    /// <summary>
    /// 系统用户权限仓储
    /// </summary>
    public class SystemUserPermissionRepository : ISystemUserPermissionRepository
    {
        private readonly IDbContext _dbContext;

        public SystemUserPermissionRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取用户菜单权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Privilege> GetPrivileges(Guid userId, int objectTypeCode)
        {
            Sql s = Sql.Builder.Append(@"select distinct a.* from Privileges a
            inner join RoleObjectAccess b on a.PrivilegeId = b.ObjectId
            where b.ObjectTypeCode=@0 and (a.AuthorizationEnabled = 0 or b.RoleId in (select RoleId from SystemUserRoles where SystemUserId = @1))", objectTypeCode, userId);
            return new DataRepositoryBase<Privilege>(_dbContext).ExecuteQuery(s);
        }

        /// <summary>
        /// 获取用户菜单权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="areaName"></param>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public Privilege GetAuthPrivilege(Guid userId, string areaName, string className, string methodName, int objectTypeCode)
        {
            Sql s = Sql.Builder.Append(@"select distinct c.* from RoleObjectAccess a
            inner join SystemUserRoles b on a.RoleId = b.RoleId
            inner join Privileges c on a.ObjectId = c.PrivilegeId
            where a.ObjectTypeCode=@4 and b.SystemUserId=@0
            and c.SystemName = @1 and c.ClassName = @2 and c.MethodName = @3", userId, areaName, className, methodName, objectTypeCode);

            return new DataRepositoryBase<Privilege>(_dbContext).ExecuteFind(s);
        }

        public Privilege GetAuthPrivilege(Guid userId, string url, int objectTypeCode)
        {
            Sql s = Sql.Builder.Append(@"select distinct c.* from RoleObjectAccess a
            inner join SystemUserRoles b on a.RoleId = b.RoleId
            inner join Privileges c on a.ObjectId = c.PrivilegeId
            where a.ObjectTypeCode=@2 and b.SystemUserId=@0
            and c.Url = @1", userId, url, objectTypeCode);

            return new DataRepositoryBase<Privilege>(_dbContext).ExecuteFind(s);
        }

        /// <summary>
        /// 获取用户无读取权限的字段
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="securityFields"></param>
        /// <returns></returns>
        public List<Guid> GetNoneReadFields(Guid userId, List<Guid> securityFields, int objectTypeCode)
        {
            if (securityFields.IsEmpty())
            {
                return securityFields;
            }
            List<Guid> result;
            Sql s = Sql.Builder.Append("select distinct b.AttributeId from RoleObjectAccess a")
                .Append("inner join Attribute b on a.ObjectId = b.AttributeId")
                .Append("inner join SystemUserRoles c on a.RoleId = c.RoleId")
                .Append("where a.AccessRightsMask = 1 and c.SystemUserId=@0", userId)
                .Append("and a.ObjectId in(@0)", securityFields.Select(x => (object)x).ToArray())
                .Append("and a.ObjectTypeCode = @0", objectTypeCode);
            var data = new DataRepositoryBase<dynamic>(_dbContext).ExecuteQuery(s);
            if (data.NotEmpty())
            {
                result = securityFields.Where(x => !data.Select(f => f.AttributeId).Contains(x)).ToList();
            }
            else
            {
                result = securityFields;
            }
            return result;
        }

        /// <summary>
        /// 获取用户无编辑权限的字段
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="securityFields"></param>
        /// <returns></returns>
        public List<Guid> GetNoneEditFields(Guid userId, List<Guid> securityFields, int objectTypeCode)
        {
            if (securityFields.IsEmpty())
            {
                return securityFields;
            }
            List<Guid> result;
            Sql s = Sql.Builder.Append("select distinct b.AttributeId from RoleObjectAccess a")
                .Append("inner join Attribute b on a.ObjectId = b.AttributeId")
                .Append("inner join SystemUserRoles c on a.RoleId = c.RoleId")
                .Append("where a.AccessRightsMask = 1 and c.SystemUserId=@0", userId)
                .Append("and a.ObjectId in(@0)", securityFields.Select(x => (object)x).ToArray())
                .Append("and a.ObjectTypeCode = @0", objectTypeCode);
            var data = new DataRepositoryBase<dynamic>(_dbContext).ExecuteQuery(s);
            if (data.NotEmpty())
            {
                result = securityFields.Where(x => !data.Select(f => f.AttributeId).Contains(x)).ToList();
            }
            else
            {
                result = securityFields;
            }
            return result;
        }
    }
}