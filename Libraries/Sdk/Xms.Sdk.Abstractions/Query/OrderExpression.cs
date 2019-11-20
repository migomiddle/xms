using System.Runtime.Serialization;

namespace Xms.Sdk.Abstractions.Query
{
    public sealed class OrderExpression
    {
        [DataMember]
        public string AttributeName { get; set; }

        [DataMember]
        public OrderType OrderType { get; set; }

        public OrderExpression()
        {
        }

        public OrderExpression(string attributeName, OrderType orderType)
        {
            this.AttributeName = attributeName;
            this.OrderType = orderType;
        }
    }
}