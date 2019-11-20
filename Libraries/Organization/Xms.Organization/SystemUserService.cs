using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Organization.Data;
using Xms.Organization.Domain;

namespace Xms.Organization
{
    /// <summary>
    /// 系统用户服务
    /// </summary>
    public class SystemUserService : ISystemUserService
    {
        private readonly ISystemUserRepository _repository;

        public SystemUserService(ISystemUserRepository repository)
        {
            _repository = repository;
        }

        public bool ExistsLoginName(string loginName, Guid? currentUserId)
        {
            return _repository.Exists(x => x.LoginName == loginName && x.SystemUserId != currentUserId);
        }

        public bool Create(SystemUser entity)
        {
            return _repository.Create(entity);
        }

        public bool Update(SystemUser entity)
        {
            return _repository.Update(entity);
        }

        public SystemUser GetUserByLoginName(string loginName)
        {
            return _repository.Find(x => x.LoginName == loginName);
        }

        public SystemUser GetUserByLoginNameAndPassword(string loginName, string password, string salt = "")
        {
            if (salt.IsNotEmpty())
            {
                password = SecurityHelper.MD5(password + salt);
            }
            return _repository.Find(x => x.LoginName == loginName && x.Password == password);
        }

        public bool IsValidePassword(string inputPassword, string salt, string password)
        {
            return SecurityHelper.MD5(inputPassword + salt).IsCaseInsensitiveEqual(password);
        }

        public SystemUser FindById(Guid id)
        {
            return _repository.FindById(id);
        }

        public SystemUser Find(Expression<Func<SystemUser, bool>> predicate)
        {
            return _repository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            //检查是否存在引用
            return _repository.DeleteById(id);
        }

        public bool DeleteById(List<Guid> ids)
        {
            var flag = true;
            foreach (var id in ids)
            {
                flag = this.DeleteById(id);
            }
            return flag;
            //return _repository.DeleteById(ids);
        }

        public PagedList<SystemUser> QueryPaged(Func<QueryDescriptor<SystemUser>, QueryDescriptor<SystemUser>> container)
        {
            QueryDescriptor<SystemUser> q = container(QueryDescriptorBuilder.Build<SystemUser>());

            return _repository.QueryPaged(q);
        }

        public List<SystemUser> Query(Func<QueryDescriptor<SystemUser>, QueryDescriptor<SystemUser>> container)
        {
            QueryDescriptor<SystemUser> q = container(QueryDescriptorBuilder.Build<SystemUser>());

            return _repository.Query(q)?.ToList();
        }

        public bool Update(Func<UpdateContext<SystemUser>, UpdateContext<SystemUser>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<SystemUser>());
            return _repository.Update(ctx);
        }

        /// <summary>
        /// 更新用户最后访问时间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateLastVisitTime(Guid userId)
        {
            return this.Update(n => n.Set(f => f.LastVisitTime, DateTime.Now).Where(f => f.SystemUserId == userId));
        }

        /// <summary>
        /// 更新用户最后登录时间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateLastLoginTime(Guid userId)
        {
            return this.Update(n => n.Set(f => f.LastLoginTime, DateTime.Now).Where(f => f.SystemUserId == userId));
        }
    }
}