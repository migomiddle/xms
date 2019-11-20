namespace Xms.Sdk.Abstractions.Query
{
    public sealed class AttributeAlias
    {
        public string Name { get; set; }

        public string EntityName { get; set; }
        public string EntityAlias { get; set; }

        public string Alias { get; set; }
    }
}