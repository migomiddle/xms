using System;

namespace Xms.Solution.Abstractions
{
    public interface ISolutionComponentExporter
    {
        string GetXml(Guid solutionId);
    }
}