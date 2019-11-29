using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Flow.Domain;

namespace Xms.Flow
{
    public interface IWorkFlowProcessFinder
    {
        WorkFlowProcess Find(Expression<Func<WorkFlowProcess, bool>> predicate);

        WorkFlowProcess FindById(Guid id);

        WorkFlowProcess GetCurrentStep(Guid WorkFlowInstanceId, Guid handlerId);

        WorkFlowProcess GetLastHandledStep(Guid WorkFlowInstanceId, Guid handlerId);

        List<WorkFlowProcess> Query(Func<QueryDescriptor<WorkFlowProcess>, QueryDescriptor<WorkFlowProcess>> container);

        PagedList<dynamic> QueryApplyHandledList(Guid applierId, int page, int pageSize, Guid? entityid);

        PagedList<dynamic> QueryApplyHandlingList(Guid applierId, int page, int pageSize, Guid? entityid);

        PagedList<dynamic> QueryHandledList(Guid handlerId, int page, int pageSize, Guid? entityid);

        PagedList<dynamic> QueryHandlingList(Guid handlerId, int page, int pageSize, Guid? entityid);

        long QueryHandledCount(Guid handlerId, Guid? entityid);

        long QueryHandlingCount(Guid handlerId, Guid? entityid);

        long QueryApplyHandledCount(Guid applierId, Guid? entityid);

        long QueryApplyHandlingCount(Guid applierId, Guid? entityid);

        PagedList<WorkFlowProcess> QueryPaged(Func<QueryDescriptor<WorkFlowProcess>, QueryDescriptor<WorkFlowProcess>> container);
    }
}