namespace Xms.Business.Filter
{
    public static class FilterRuleDefaults
    {
        public const string ModuleName = "FilterRule";

        public static string AssemblyName
        {
            get
            {
                return typeof(FilterRuleExecutor).Namespace + ".dll";
            }
        }

        public static string PluginClassName
        {
            get
            {
                return typeof(FilterRuleExecutor).FullName + "," + typeof(FilterRuleExecutor).Namespace;
            }
        }
    }
}