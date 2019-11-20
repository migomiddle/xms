using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Flow.Domain;

namespace Xms.Flow
{
    public interface IWorkFlowStepService
    {
        bool Create(WorkFlowStep entity);

        bool CreateMany(IList<WorkFlowStep> entities);

        bool DeleteById(params Guid[] id);

        bool DeleteByParentId(Guid parentid);

        WorkFlowStep Find(Expression<Func<WorkFlowStep, bool>> predicate);

        WorkFlowStep FindById(Guid id);

        List<WorkFlowStep> Query(Func<QueryDescriptor<WorkFlowStep>, QueryDescriptor<WorkFlowStep>> container);

        PagedList<WorkFlowStep> QueryPaged(Func<QueryDescriptor<WorkFlowStep>, QueryDescriptor<WorkFlowStep>> container);

        bool Update(Func<UpdateContext<WorkFlowStep>, UpdateContext<WorkFlowStep>> context);

        bool Update(WorkFlowStep entity);
    }
}