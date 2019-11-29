using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Flow.Domain;

namespace Xms.Flow
{
    public interface IWorkFlowInstanceService
    {
        void BeginTransaction();

        void CompleteTransaction();

        bool Create(WorkFlowInstance entity);

        bool CreateMany(List<WorkFlowInstance> entities);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        bool DeleteByObjectId(Guid entityId, Guid objectId);

        bool DeleteByParentId(Guid parentid);

        WorkFlowInstance Find(Expression<Func<WorkFlowInstance, bool>> predicate);

        WorkFlowInstance FindById(Guid id);

        List<WorkFlowInstance> Top(Func<QueryDescriptor<WorkFlowInstance>, QueryDescriptor<WorkFlowInstance>> container);

        List<WorkFlowInstance> Query(Func<QueryDescriptor<WorkFlowInstance>, QueryDescriptor<WorkFlowInstance>> container);

        PagedList<WorkFlowInstance> QueryPaged(Func<QueryDescriptor<WorkFlowInstance>, QueryDescriptor<WorkFlowInstance>> container);

        void RollBackTransaction();

        bool Update(Func<UpdateContext<WorkFlowInstance>, UpdateContext<WorkFlowInstance>> context);

        bool Update(WorkFlowInstance entity);
    }
}