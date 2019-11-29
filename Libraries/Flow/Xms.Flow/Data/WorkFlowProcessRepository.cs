using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data;
using Xms.Flow.Domain;
using Xms.Infrastructure.Utility;

namespace Xms.Flow.Data
{
    /// <summary>
    /// 流程执行步骤仓储
    /// </summary>
    public class WorkFlowProcessRepository : DefaultRepository<WorkFlowProcess>, IWorkFlowProcessRepository
    {
        public WorkFlowProcessRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region implements

        public long QueryHandledCount(Guid handlerId, Guid? entityid)
        {
            string s = @"select count(1) as Count
            from WorkFlowProcess a
            inner join WorkFlowInstance b on a.WorkFlowInstanceId = b.WorkFlowInstanceId
            inner join Entity c on b.EntityId = c.EntityId
            inner join SystemUser d on b.ApplicantId = d.SystemUserId
            inner join WorkFlow e on b.WorkFlowId = e.WorkFlowId
            where a.HandlerId=@0 and a.StateCode in(2,3)
            ";
            if (entityid.HasValue && !entityid.Value.Equals(Guid.Empty))
            {
                s += " and b.EntityId='" + entityid.Value + "'";
            }
            var dataRepository = new DataRepositoryBase<dynamic>(DbContext);
            var result = dataRepository.ExecuteQuery(s, handlerId);
            if (result.NotEmpty())
            {
                var value = (result[0] as IDictionary<string, object>).Values.First();
                return value != null ? long.Parse(value.ToString()) : 0;
            }
            return 0;
        }

        public PagedList<dynamic> QueryHandledList(Guid handlerId, int page, int pageSize, Guid? entityid)
        {
            string s = @"select a.*,e.Name AS WorkFlowName,b.ApplicantId,b.EntityId,b.ObjectId,b.Description as ApplyDescription,c.LocalizedName as EntityLocalizedName,d.Name as ApplicantIdName
            from WorkFlowProcess a
            inner join WorkFlowInstance b on a.WorkFlowInstanceId = b.WorkFlowInstanceId
            inner join Entity c on b.EntityId = c.EntityId
            inner join SystemUser d on b.ApplicantId = d.SystemUserId
            inner join WorkFlow e on b.WorkFlowId = e.WorkFlowId
            where a.HandlerId=@0 and a.StateCode in(2,3)
            ";
            if (entityid.HasValue && !entityid.Value.Equals(Guid.Empty))
            {
                s += " and b.EntityId='" + entityid.Value + "'";
            }
            s += " order by a.HandleTime desc";
            var dataRepository = new DataRepositoryBase<dynamic>(DbContext);
            return dataRepository.ExecuteQueryPaged(page, pageSize, s, handlerId);
        }

        public long QueryHandlingCount(Guid handlerId, Guid? entityid)
        {
            string s = @"select count(1) as Count
            from WorkFlowProcess a
            inner join WorkFlowInstance b on a.WorkFlowInstanceId = b.WorkFlowInstanceId
            inner join Entity c on b.EntityId = c.EntityId
            inner join SystemUser d on b.ApplicantId = d.SystemUserId
            inner join WorkFlow e on b.WorkFlowId = e.WorkFlowId
            where a.HandlerId=@0 and a.StateCode = 1
            ";
            if (entityid.HasValue && !entityid.Value.Equals(Guid.Empty))
            {
                s += " and b.EntityId='" + entityid.Value + "'";
            }
            var dataRepository = new DataRepositoryBase<dynamic>(DbContext);
            var result = dataRepository.ExecuteQuery(s, handlerId);
            if (result.NotEmpty())
            {
                var value = (result[0] as IDictionary<string, object>).Values.First();
                return value != null ? long.Parse(value.ToString()) : 0;
            }
            return 0;
        }

        public PagedList<dynamic> QueryHandlingList(Guid handlerId, int page, int pageSize, Guid? entityid)
        {
            string s = @"select a.*,e.Name AS WorkFlowName,b.ApplicantId,b.EntityId,b.ObjectId,b.Description as ApplyDescription,c.LocalizedName as EntityLocalizedName,d.Name as ApplicantIdName
            from WorkFlowProcess a
            inner join WorkFlowInstance b on a.WorkFlowInstanceId = b.WorkFlowInstanceId
            inner join Entity c on b.EntityId = c.EntityId
            inner join SystemUser d on b.ApplicantId = d.SystemUserId
            inner join WorkFlow e on b.WorkFlowId = e.WorkFlowId
            where a.HandlerId=@0 and a.StateCode = 1
            ";
            if (entityid.HasValue && !entityid.Value.Equals(Guid.Empty))
            {
                s += " and b.EntityId='" + entityid.Value + "'";
            }
            s += " order by a.StartTime desc";

            var dataRepository = new DataRepositoryBase<dynamic>(DbContext);
            return dataRepository.ExecuteQueryPaged(page, pageSize, s, handlerId);
        }

        public long QueryApplyHandledCount(Guid handlerId, Guid? entityid)
        {
            string s = @"select count(1) as Count
            from WorkFlowInstance b
            inner join Entity c on b.EntityId = c.EntityId
            inner join SystemUser d on b.ApplicantId = d.SystemUserId
            inner join WorkFlow e on b.WorkFlowId = e.WorkFlowId
            where b.ApplicantId=@0 and b.StateCode > 1
            ";
            if (entityid.HasValue && !entityid.Value.Equals(Guid.Empty))
            {
                s += " and b.EntityId='" + entityid.Value + "'";
            }
            var dataRepository = new DataRepositoryBase<dynamic>(DbContext);
            var result = dataRepository.ExecuteQuery(s, handlerId);
            if (result.NotEmpty())
            {
                var value = (result[0] as IDictionary<string, object>).Values.First();
                return value != null ? long.Parse(value.ToString()) : 0;
            }
            return 0;
        }

        public PagedList<dynamic> QueryApplyHandledList(Guid applierId, int page, int pageSize, Guid? entityid)
        {
            string s = @"select b.*,c.LocalizedName as EntityLocalizedName,d.Name as ApplicantIdName,e.Name AS WorkFlowName
            from WorkFlowInstance b
            inner join Entity c on b.EntityId = c.EntityId
            inner join SystemUser d on b.ApplicantId = d.SystemUserId
            inner join WorkFlow e on b.WorkFlowId = e.WorkFlowId
            where b.ApplicantId=@0 and b.StateCode > 1
            ";
            if (entityid.HasValue && !entityid.Value.Equals(Guid.Empty))
            {
                s += " and b.EntityId='" + entityid.Value + "'";
            }
            s += " order by b.CompletedOn desc";

            var dataRepository = new DataRepositoryBase<dynamic>(DbContext);
            return dataRepository.ExecuteQueryPaged(page, pageSize, s, applierId);
        }

        public long QueryApplyHandlingCount(Guid handlerId, Guid? entityid)
        {
            string s = @"select count(1) as Count
            from WorkFlowInstance b
            inner join Entity c on b.EntityId = c.EntityId
            inner join SystemUser d on b.ApplicantId = d.SystemUserId
            inner join WorkFlow e on b.WorkFlowId = e.WorkFlowId
            where b.ApplicantId=@0 and b.StateCode = 1
            ";
            if (entityid.HasValue && !entityid.Value.Equals(Guid.Empty))
            {
                s += " and b.EntityId='" + entityid.Value + "'";
            }
            var dataRepository = new DataRepositoryBase<dynamic>(DbContext);
            var result = dataRepository.ExecuteQuery(s, handlerId);
            if (result.NotEmpty())
            {
                var value = (result[0] as IDictionary<string, object>).Values.First();
                return value != null ? long.Parse(value.ToString()) : 0;
            }
            return 0;
        }

        public PagedList<dynamic> QueryApplyHandlingList(Guid applierId, int page, int pageSize, Guid? entityid)
        {
            string s = @"select b.*,c.LocalizedName as EntityLocalizedName,d.Name as ApplicantIdName,e.Name AS WorkFlowName
            from WorkFlowInstance b
            inner join Entity c on b.EntityId = c.EntityId
            inner join SystemUser d on b.ApplicantId = d.SystemUserId
            inner join WorkFlow e on b.WorkFlowId = e.WorkFlowId
            where b.ApplicantId=@0 and b.StateCode = 1
            ";
            if (entityid.HasValue && !entityid.Value.Equals(Guid.Empty))
            {
                s += " and b.EntityId='" + entityid.Value + "'";
            }
            s += " order by b.CreatedOn desc";

            var dataRepository = new DataRepositoryBase<dynamic>(DbContext);
            return dataRepository.ExecuteQueryPaged(page, pageSize, s, applierId);
        }

        #endregion implements
    }
}