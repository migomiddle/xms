namespace Xms.Core.Context
{
    /// <summary>
    /// 表达式参数
    /// </summary>
    public class QueryParameter
    {
        public QueryParameter()
        {
        }

        public QueryParameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }

        public object Value { get; set; }
    }
}