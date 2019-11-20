using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Context;
using Xms.Core;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Logging.DataLog.Data;
using Xms.Logging.DataLog.Domain;
using Xms.Schema.Extensions;
using Xms.Sdk.Abstractions;

namespace Xms.Logging.DataLog
{
    /// <summary>
    /// 实体数据日志服务
    /// </summary>
    public class EntityLogService : IEntityLogService
    {
        private readonly IEntityLogRepository _entityLogRepository;
        private readonly IAppContext _appContext;
        private readonly ICurrentUser _currentUser;

        public EntityLogService(IAppContext appContext
            , IEntityLogRepository entityLogRepository)
        {
            _appContext = appContext;
            _currentUser = _appContext.GetFeature<ICurrentUser>();
            _entityLogRepository = entityLogRepository;
        }

        #region sync

        public bool Create(EntityLog entity)
        {
            return _entityLogRepository.Create(entity);
        }

        public bool CreatedLog(Entity data, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas)
        {
            if (!entityMetadata.LogEnabled)
            {
                return false;
            }
            EntityLog entity = new EntityLog
            {
                EntityId = entityMetadata.EntityId,
                EntityLogId = Guid.NewGuid(),
                OperationType = OperationTypeEnum.Create,
                UserId = _currentUser.SystemUserId,
                OrganizationId = _currentUser.OrganizationId,
                CreatedOn = DateTime.Now,
                RecordId = data.GetIdValue(),
                AttributeMask = string.Join(",", data.Keys)
            };
            var datas = new List<EntityLogChangeData>();
            foreach (var item in data)
            {
                var attr = attributeMetadatas.Find(n => n.Name.IsCaseInsensitiveEqual(item.Key));
                if (!item.Key.IsCaseInsensitiveEqual("createdon") && !item.Key.IsCaseInsensitiveEqual("createdby")
                    && !item.Key.IsCaseInsensitiveEqual("modifiedon") && !item.Key.IsCaseInsensitiveEqual("modifiedby")
                         && !item.Key.IsCaseInsensitiveEqual("versionnumber"))
                {
                    if (attr != null && (attr.TypeIsText() || attr.TypeIsNText()))
                    {
                        continue;
                    }

                    datas.Add(new EntityLogChangeData() { Name = item.Key, Original = string.Empty, Value = item.Value != null ? item.Value.ToString() : "" });
                }
            }
            entity.ChangeData = datas.SerializeToJson();
            return Create(entity);
        }

        public bool UpdatedLog(Entity originData, Entity newData, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas)
        {
            if (!entityMetadata.LogEnabled)
            {
                return false;
            }
            EntityLog entity = new EntityLog
            {
                EntityId = entityMetadata.EntityId,
                EntityLogId = Guid.NewGuid(),
                OperationType = OperationTypeEnum.Update,
                UserId = _currentUser.SystemUserId,
                OrganizationId = _currentUser.OrganizationId,
                CreatedOn = DateTime.Now,
                RecordId = newData.GetIdValue(),
                AttributeMask = string.Join(",", newData.Keys)
            };
            var datas = new List<EntityLogChangeData>();
            foreach (var item in newData)
            {
                var originValue = originData.TryGetValue(item.Key, out object value) ? value.ToString() : "";
                if (!originValue.IsCaseInsensitiveEqual(item.Value != null ? item.Value.ToString() : ""))
                {
                    var attr = attributeMetadatas.Find(n => n.Name.IsCaseInsensitiveEqual(item.Key));
                    if (!item.Key.IsCaseInsensitiveEqual("createdon") && !item.Key.IsCaseInsensitiveEqual("createdby")
                        && !item.Key.IsCaseInsensitiveEqual("modifiedon") && !item.Key.IsCaseInsensitiveEqual("modifiedby")
                         && !item.Key.IsCaseInsensitiveEqual("versionnumber"))
                    {
                        if (attr != null && (attr.TypeIsText() || attr.TypeIsNText()))
                        {
                            continue;
                        }

                        datas.Add(new EntityLogChangeData() { Name = item.Key, Original = originData[item.Key].ToString(), Value = item.Value != null ? item.Value.ToString() : "" });
                    }
                }
            }
            entity.ChangeData = datas.SerializeToJson();
            return Create(entity);
        }

        public bool DeletedLog(Entity data, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas)
        {
            if (!entityMetadata.LogEnabled)
            {
                return false;
            }
            EntityLog entity = new EntityLog
            {
                EntityId = entityMetadata.EntityId,
                EntityLogId = Guid.NewGuid(),
                OperationType = OperationTypeEnum.Delete,
                UserId = _currentUser.SystemUserId,
                OrganizationId = _currentUser.OrganizationId,
                CreatedOn = DateTime.Now,
                RecordId = data.GetIdValue(),
                AttributeMask = string.Join(",", data.Keys)
            };
            var datas = new List<EntityLogChangeData>();
            foreach (var item in data)
            {
                if (!item.Key.IsCaseInsensitiveEqual("versionnumber"))
                {
                    var attr = attributeMetadatas.Find(n => n.Name.IsCaseInsensitiveEqual(item.Key));
                    if (attr != null && (attr.TypeIsText() || attr.TypeIsNText()))
                    {
                        continue;
                    }
                    datas.Add(new EntityLogChangeData() { Name = item.Key, Value = string.Empty, Original = item.Value != null ? item.Value.ToString() : "" });
                }
            }
            entity.ChangeData = datas.SerializeToJson();
            return Create(entity);
        }

        public bool SharedLog(Entity data, Schema.Domain.Entity entityMetadata)
        {
            if (!entityMetadata.LogEnabled)
            {
                return false;
            }
            EntityLog entity = new EntityLog
            {
                EntityId = entityMetadata.EntityId,
                EntityLogId = Guid.NewGuid(),
                OperationType = OperationTypeEnum.Share,
                UserId = _currentUser.SystemUserId,
                OrganizationId = _currentUser.OrganizationId,
                CreatedOn = DateTime.Now,
                RecordId = data.GetIdValue()
            };
            return Create(entity);
        }

        public bool AssignedLog(Entity data, OwnerObject from, OwnerObject to, Schema.Domain.Entity entityMetadata)
        {
            if (!entityMetadata.LogEnabled)
            {
                return false;
            }
            EntityLog entity = new EntityLog
            {
                EntityId = entityMetadata.EntityId,
                EntityLogId = Guid.NewGuid(),
                OperationType = OperationTypeEnum.Assign,
                UserId = _currentUser.SystemUserId,
                OrganizationId = _currentUser.OrganizationId,
                CreatedOn = DateTime.Now,
                RecordId = data.GetIdValue()
            };
            var datas = new List<EntityLogChangeData>
            {
                new EntityLogChangeData() { Name = "ownerid", Original = from.SerializeToJson(), Value = to.SerializeToJson() }
            };
            entity.ChangeData = datas.SerializeToJson();
            return Create(entity);
        }

        public bool MergedLog(Entity merged, Entity target, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas)
        {
            if (!entityMetadata.LogEnabled)
            {
                return false;
            }
            EntityLog entity = new EntityLog
            {
                EntityId = entityMetadata.EntityId,
                EntityLogId = Guid.NewGuid(),
                OperationType = OperationTypeEnum.Merge,
                UserId = _currentUser.SystemUserId,
                OrganizationId = _currentUser.OrganizationId,
                CreatedOn = DateTime.Now,
                RecordId = target.GetIdValue(),
                AttributeMask = string.Join(",", target.Keys)
            };
            var datas = new List<EntityLogChangeData>();
            foreach (var item in target)
            {
                var originValue = merged.TryGetValue(item.Key, out object value) ? value.ToString() : "";
                if (!originValue.ToString().IsCaseInsensitiveEqual(item.Value != null ? item.Value.ToString() : ""))
                {
                    var attr = attributeMetadatas.Find(n => n.Name.IsCaseInsensitiveEqual(item.Key));
                    if (!item.Key.IsCaseInsensitiveEqual("createdon") && !item.Key.IsCaseInsensitiveEqual("createdby")
                        && !item.Key.IsCaseInsensitiveEqual("modifiedon") && !item.Key.IsCaseInsensitiveEqual("modifiedby")
                         && !item.Key.IsCaseInsensitiveEqual("versionnumber"))
                    {
                        if (attr != null && (attr.TypeIsText() || attr.TypeIsNText()))
                        {
                            continue;
                        }

                        datas.Add(new EntityLogChangeData() { Name = item.Key, Original = merged[item.Key].ToString(), Value = item.Value != null ? item.Value.ToString() : "" });
                    }
                }
            }
            entity.ChangeData = datas.SerializeToJson();
            return Create(entity);
        }

        public EntityLog FindById(Guid id)
        {
            return _entityLogRepository.FindById(id);
        }

        public PagedList<EntityLog> QueryPaged(Func<QueryDescriptor<EntityLog>, QueryDescriptor<EntityLog>> container)
        {
            QueryDescriptor<EntityLog> q = container(QueryDescriptorBuilder.Build<EntityLog>());

            return _entityLogRepository.QueryPaged(q);
        }

        public List<EntityLog> Query(Func<QueryDescriptor<EntityLog>, QueryDescriptor<EntityLog>> container)
        {
            QueryDescriptor<EntityLog> q = container(QueryDescriptorBuilder.Build<EntityLog>());

            return _entityLogRepository.Query(q)?.ToList();
        }

        public void Clear()
        {
            _entityLogRepository.Clear();
        }

        public void Clear(Guid entityId)
        {
            _entityLogRepository.Clear(entityId);
        }

        #endregion sync
    }
}