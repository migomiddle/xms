namespace Xms.Business.DataAnalyse.Report
{
    public class ReportDefaults
    {
        public const string ModuleName = "Report";

        public static Solution.Abstractions.SolutionComponentDescriptor ComponentDescriptor
        {
            get
            {
                return Solution.Abstractions.SolutionComponentCollection.GetDescriptor(ModuleName);
            }
        }
    }
}