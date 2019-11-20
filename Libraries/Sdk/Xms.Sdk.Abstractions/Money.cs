namespace Xms.Sdk.Abstractions
{
    public sealed class Money
    {
        public Money(decimal value)
        {
            this.Value = value;
        }

        public decimal Value { get; set; }
    }
}