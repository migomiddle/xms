using System;
using Xms.Flow.Domain;

namespace Xms.Flow
{
    public interface IWorkFlowUpdater
    {
        bool Update(WorkFlow entity);

        bool UpdateAuthorization(bool isAuthorization, params Guid[] id);

        bool UpdateState(bool isEnabled, params Guid[] id);
    }
}