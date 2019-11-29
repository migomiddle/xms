using Microsoft.Extensions.Options;

namespace Xms.Data.Abstractions
{
    /// <summary>
    /// 基础数据库参数
    /// </summary>
    public class DataBaseOptions : IOptions<DataBaseOptions>, IDataProviderOptions
    {
        public string DbType { get; set; }
        public int CommandTimeOut { get; set; } = 10;
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; } = "System.Data.SqlClient";

        public DataBaseOptions Value => this;
    }
}