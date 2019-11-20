using System;

namespace Xms.Solution
{
    public interface ISolutionExporter
    {
        bool Export(Guid solutionId, out string file);
    }
}