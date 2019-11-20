using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Flow.Domain;

namespace Xms.Flow
{
    public interface IWorkFlowProcessLogService
    {
        bool Create(WorkFlowProcessLog entity);

        bool CreateMany(IEnumerable<WorkFlowProcessLog> entities);

        bool DeleteById(Guid id);

        bool DeleteById(IEnumerable<Guid> ids);

        WorkFlowProcessLog Find(Expression<Func<WorkFlowProcessLog, bool>> predicate);

        WorkFlowProcessLog FindById(Guid id);

        List<WorkFlowProcessLog> Query(Func<QueryDescriptor<WorkFlowProcessLog>, QueryDescriptor<WorkFlowProcessLog>> container);

        PagedList<WorkFlowProcessLog> QueryPaged(Func<QueryDescriptor<WorkFlowProcessLog>, QueryDescriptor<WorkFlowProcessLog>> container);

        bool Update(Func<UpdateContext<WorkFlowProcessLog>, UpdateContext<WorkFlowProcessLog>> context);

        bool Update(WorkFlowProcessLog entity);
    }
}