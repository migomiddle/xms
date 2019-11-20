using System;
using System.Collections.Generic;

namespace Xms.QueryView
{
    public interface IQueryViewImporter
    {
        bool Import(Guid solutionId, List<Domain.QueryView> queryViews);
    }
}