using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Core;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Event.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Security.DataAuthorization.Data;
using Xms.Security.Domain;

namespace Xms.Security.DataAuthorization
{
    /// <summary>
    /// 实体权限服务
    /// </summary>
    public class EntityPermissionService : IEntityPermissionService, ICascadeDelete<Schema.Domain.Entity>
    {
        private readonly IEntityPermissionRepository _entityPermissionRepository;

        public EntityPermissionService(IEntityPermissionRepository entityPermissionRepository)
        {
            _entityPermissionRepository = entityPermissionRepository;
        }

        public bool Create(EntityPermission entity)
        {
            return _entityPermissionRepository.Create(entity);
        }

        public bool CreateMany(List<EntityPermission> entities)
        {
            return _entityPermissionRepository.CreateMany(entities);
        }

        public bool CreateDefaultPermissions(Schema.Domain.Entity entity)
        {
            return _entityPermissionRepository.CreateMany(GetDefaultPermissions(entity));
        }

        public bool Update(EntityPermission entity)
        {
            return _entityPermissionRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<EntityPermission>, UpdateContext<EntityPermission>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<EntityPermission>());
            return _entityPermissionRepository.Update(ctx);
        }

        public EntityPermission FindById(Guid id)
        {
            return _entityPermissionRepository.FindById(id);
        }

        public EntityPermission Find(Expression<Func<EntityPermission, bool>> predicate)
        {
            return _entityPermissionRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            return _entityPermissionRepository.DeleteById(id);
        }

        public bool DeleteById(IEnumerable<Guid> ids)
        {
            return _entityPermissionRepository.DeleteMany(ids);
        }

        public PagedList<EntityPermission> QueryPaged(Func<QueryDescriptor<EntityPermission>, QueryDescriptor<EntityPermission>> container)
        {
            QueryDescriptor<EntityPermission> q = container(QueryDescriptorBuilder.Build<EntityPermission>());

            return _entityPermissionRepository.QueryPaged(q);
        }

        public List<EntityPermission> Query(Func<QueryDescriptor<EntityPermission>, QueryDescriptor<EntityPermission>> container)
        {
            QueryDescriptor<EntityPermission> q = container(QueryDescriptorBuilder.Build<EntityPermission>());

            return _entityPermissionRepository.Query(q)?.ToList();
        }

        private List<EntityPermission> GetDefaultPermissions(Schema.Domain.Entity entity)
        {
            List<EntityPermission> entityPermissions = new List<EntityPermission>();
            entityPermissions.Add(new EntityPermission() { EntityPermissionId = Guid.NewGuid(), EntityId = entity.EntityId, AccessRight = AccessRightValue.Read, Name = entity.Name + ".read", State = RecordState.Enabled, Description = "读取" + entity.LocalizedName });
            entityPermissions.Add(new EntityPermission() { EntityPermissionId = Guid.NewGuid(), EntityId = entity.EntityId, AccessRight = AccessRightValue.Create, Name = entity.Name + ".create", State = RecordState.Enabled, Description = "创建" + entity.LocalizedName });
            entityPermissions.Add(new EntityPermission() { EntityPermissionId = Guid.NewGuid(), EntityId = entity.EntityId, AccessRight = AccessRightValue.Update, Name = entity.Name + ".update", State = RecordState.Enabled, Description = "更新" + entity.LocalizedName });
            entityPermissions.Add(new EntityPermission() { EntityPermissionId = Guid.NewGuid(), EntityId = entity.EntityId, AccessRight = AccessRightValue.Delete, Name = entity.Name + ".delete", State = RecordState.Enabled, Description = "删除" + entity.LocalizedName });
            entityPermissions.Add(new EntityPermission() { EntityPermissionId = Guid.NewGuid(), EntityId = entity.EntityId, AccessRight = AccessRightValue.Export, Name = entity.Name + ".export", State = RecordState.Enabled, Description = "导出" + entity.LocalizedName });
            entityPermissions.Add(new EntityPermission() { EntityPermissionId = Guid.NewGuid(), EntityId = entity.EntityId, AccessRight = AccessRightValue.Import, Name = entity.Name + ".import", State = RecordState.Enabled, Description = "导入" + entity.LocalizedName });
            entityPermissions.Add(new EntityPermission() { EntityPermissionId = Guid.NewGuid(), EntityId = entity.EntityId, AccessRight = AccessRightValue.Share, Name = entity.Name + ".share", State = RecordState.Enabled, Description = "共享" + entity.LocalizedName });
            entityPermissions.Add(new EntityPermission() { EntityPermissionId = Guid.NewGuid(), EntityId = entity.EntityId, AccessRight = AccessRightValue.Assign, Name = entity.Name + ".assign", State = RecordState.Enabled, Description = "分派" + entity.LocalizedName });
            entityPermissions.Add(new EntityPermission() { EntityPermissionId = Guid.NewGuid(), EntityId = entity.EntityId, AccessRight = AccessRightValue.Append, Name = entity.Name + ".append", State = RecordState.Enabled, Description = "创建" + entity.LocalizedName + "的下游单据" });
            entityPermissions.Add(new EntityPermission() { EntityPermissionId = Guid.NewGuid(), EntityId = entity.EntityId, AccessRight = AccessRightValue.AppendTo, Name = entity.Name + ".appendto", State = RecordState.Enabled, Description = "引用" + entity.LocalizedName });
            return entityPermissions;
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的实体</param>
        public void CascadeDelete(params Schema.Domain.Entity[] parent)
        {
            if (parent.IsEmpty())
            {
                return;
            }
            var entityIds = parent.Select(x => x.EntityId).ToArray();
            _entityPermissionRepository.DeleteMany(x => x.EntityId.In(entityIds));
        }
    }

    /// <summary>
    /// 实体创建事件消费者之创建默认权限
    /// </summary>
    public class CreateDefaultEntityPermissionsConsumer : IConsumer<ObjectCreatedEvent<Schema.Domain.Entity>>
    {
        private readonly IEntityPermissionService _entityPermissionService;

        public CreateDefaultEntityPermissionsConsumer(IEntityPermissionService entityPermissionService)
        {
            _entityPermissionService = entityPermissionService;
        }

        public void HandleEvent(ObjectCreatedEvent<Schema.Domain.Entity> eventMessage)
        {
            _entityPermissionService.CreateDefaultPermissions(eventMessage.Object);
        }
    }
}