using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Security.Role.Data;

namespace Xms.Security.Role
{
    /// <summary>
    /// 安全角色服务
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public bool Create(Domain.Role entity)
        {
            return _roleRepository.Create(entity);
        }

        public bool Update(Domain.Role entity)
        {
            return _roleRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<Domain.Role>, UpdateContext<Domain.Role>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<Domain.Role>());
            return _roleRepository.Update(ctx);
        }

        public Domain.Role FindById(Guid id)
        {
            return _roleRepository.FindById(id);
        }

        public List<Domain.Role> FindAll()
        {
            return _roleRepository.FindAll()?.ToList();
        }

        public bool DeleteById(Guid id)
        {
            var deletedRecord = this.FindById(id);
            if (deletedRecord != null && deletedRecord.Name.IsCaseInsensitiveEqual("administrator"))
            {
                return false;
            }
            return _roleRepository.DeleteById(id);
        }

        public bool DeleteById(IEnumerable<Guid> ids)
        {
            var deletedRecords = this.Query(n => n.Where(f => f.RoleId.In(ids)));
            if (deletedRecords.NotEmpty() && deletedRecords.Exists(n => n.Name.IsCaseInsensitiveEqual("administrator")))
            {
                ids.ToList().Remove(deletedRecords.Find(n => n.Name.IsCaseInsensitiveEqual("administrator")).RoleId);
            }
            return _roleRepository.DeleteMany(ids);
        }

        public PagedList<Domain.Role> QueryPaged(Func<QueryDescriptor<Domain.Role>, QueryDescriptor<Domain.Role>> container)
        {
            QueryDescriptor<Domain.Role> q = container(QueryDescriptorBuilder.Build<Domain.Role>());

            return _roleRepository.QueryPaged(q);
        }

        public List<Domain.Role> Query(Func<QueryDescriptor<Domain.Role>, QueryDescriptor<Domain.Role>> container)
        {
            QueryDescriptor<Domain.Role> q = container(QueryDescriptorBuilder.Build<Domain.Role>());
            return _roleRepository.Query(q)?.ToList();
        }
    }
}