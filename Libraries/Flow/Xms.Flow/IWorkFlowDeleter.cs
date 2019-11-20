using System;

namespace Xms.Flow
{
    public interface IWorkFlowDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}