namespace Xms.Schema.Abstractions
{
    public static class AttributeDefaults
    {
        public const string ModuleName = "Attribute";
        public static string[] SystemAttributes => new string[] { "createdon", "createdby", "modifiedon", "modifiedby", "ownerid", "owneridtype", "owningbusinessunit", "organizationid", "workflowid", "processstate", "stageid", "versionnumber" };
    }
}