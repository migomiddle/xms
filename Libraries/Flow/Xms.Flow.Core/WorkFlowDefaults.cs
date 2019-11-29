namespace Xms.Flow.Core
{
    public class WorkFlowDefaults
    {
        public const string ModuleName = "WorkFlow";

        public static Solution.Abstractions.SolutionComponentDescriptor ComponentDescriptor
        {
            get
            {
                return Solution.Abstractions.SolutionComponentCollection.GetDescriptor(ModuleName);
            }
        }
    }
}