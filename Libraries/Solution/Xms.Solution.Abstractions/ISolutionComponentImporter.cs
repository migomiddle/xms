using System;
using System.Collections.Generic;

namespace Xms.Solution.Abstractions
{
    public interface ISolutionComponentImporter<T>
    {
        bool Import(Guid solutionId, IList<T> components);
    }
}