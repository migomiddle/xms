namespace Xms.Business.DuplicateValidator
{
    public static class DuplicateRuleDefaults
    {
        public const string ModuleName = "DuplicateRule";

        public static string AssemblyName
        {
            get
            {
                return typeof(DuplicateRuleExecutor).Namespace + ".dll";
            }
        }

        public static string PluginClassName
        {
            get
            {
                return typeof(DuplicateRuleExecutor).FullName + "," + typeof(DuplicateRuleExecutor).Namespace;
            }
        }
    }
}