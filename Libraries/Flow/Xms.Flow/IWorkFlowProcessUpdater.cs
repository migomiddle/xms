using System;
using Xms.Core.Context;
using Xms.Flow.Domain;

namespace Xms.Flow
{
    public interface IWorkFlowProcessUpdater
    {
        bool Update(Func<UpdateContext<WorkFlowProcess>, UpdateContext<WorkFlowProcess>> context);

        bool Update(WorkFlowProcess entity);
    }
}