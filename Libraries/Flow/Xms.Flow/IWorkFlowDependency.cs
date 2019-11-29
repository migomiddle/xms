using System;
using Xms.Flow.Domain;

namespace Xms.Flow
{
    public interface IWorkFlowDependency
    {
        bool Create(WorkFlow entity);

        bool Delete(params Guid[] id);

        bool Update(WorkFlow entity);
    }
}