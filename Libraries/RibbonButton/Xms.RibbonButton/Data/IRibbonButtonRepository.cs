using System;
using Xms.Core.Context;
using Xms.Core.Data;

namespace Xms.RibbonButton.Data
{
    public interface IRibbonButtonRepository : IRepository<Domain.RibbonButton>
    {
        PagedList<Domain.RibbonButton> QueryPaged(QueryDescriptor<Domain.RibbonButton> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}