namespace Xms.Sdk.Abstractions.Query
{
    public class AttributeAggregateExpression
    {
        public string Aggregate { get; set; }
        public string EntityName { get; set; }
        public string Field { get; set; }
        public FilterExpression Filter { get; set; }
        public string RelationshipName { get; set; }
    }
}