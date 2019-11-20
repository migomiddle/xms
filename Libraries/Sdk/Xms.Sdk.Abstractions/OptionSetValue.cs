namespace Xms.Sdk.Abstractions
{
    public sealed class OptionSetValue
    {
        public OptionSetValue(int value)
        {
            this.Value = value;
        }

        public int Value { get; set; }
    }
}