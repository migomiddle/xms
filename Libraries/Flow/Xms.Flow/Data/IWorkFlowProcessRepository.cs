using System;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Flow.Domain;

namespace Xms.Flow.Data
{
    public interface IWorkFlowProcessRepository : IRepository<WorkFlowProcess>
    {
        long QueryHandledCount(Guid handlerId, Guid? entityid);

        long QueryHandlingCount(Guid handlerId, Guid? entityid);

        long QueryApplyHandledCount(Guid applierId, Guid? entityid);

        long QueryApplyHandlingCount(Guid applierId, Guid? entityid);

        PagedList<dynamic> QueryHandledList(Guid handlerId, int page, int pageSize, Guid? entityid);

        PagedList<dynamic> QueryHandlingList(Guid handlerId, int page, int pageSize, Guid? entityid);

        PagedList<dynamic> QueryApplyHandledList(Guid applierId, int page, int pageSize, Guid? entityid);

        PagedList<dynamic> QueryApplyHandlingList(Guid applierId, int page, int pageSize, Guid? entityid);
    }
}