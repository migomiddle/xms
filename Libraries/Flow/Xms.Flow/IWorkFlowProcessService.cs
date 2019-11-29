using System;
using System.Collections.Generic;
using Xms.Flow.Domain;

namespace Xms.Flow
{
    public interface IWorkFlowProcessService
    {
        bool Create(WorkFlowProcess entity);

        bool CreateMany(IEnumerable<WorkFlowProcess> entities);

        bool DeleteById(Guid id);

        bool DeleteById(IEnumerable<Guid> ids);
    }
}