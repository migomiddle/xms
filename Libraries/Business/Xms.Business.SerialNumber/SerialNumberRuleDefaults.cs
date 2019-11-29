namespace Xms.Business.SerialNumber
{
    public static class SerialNumberRuleDefaults
    {
        public const string ModuleName = "SerialNumberRule";

        public static string AssemblyName
        {
            get
            {
                return typeof(SerialNumberExecutor).Namespace + ".dll";
            }
        }

        public static string PluginClassName
        {
            get
            {
                return typeof(SerialNumberExecutor).FullName + "," + typeof(SerialNumberExecutor).Namespace;
            }
        }
    }
}