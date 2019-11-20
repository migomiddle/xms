using Xms.Flow.Domain;

namespace Xms.Flow
{
    public interface IWorkFlowCreater
    {
        bool Create(WorkFlow entity);
    }
}