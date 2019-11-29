using Xms.Module.Abstractions;

namespace Xms.Solution.Abstractions
{
    public class SolutionComponentDescriptor
    {
        public ModuleDescriptor Module { get; set; }
        public string ComponentsEndpoint { get; set; }
    }
}