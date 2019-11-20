namespace Xms.Data.Abstractions
{
    public class XmsDbConfiguration : IDataProviderOptions
    {
        public XmsDbConfiguration(Core.Org.IOrgDataServer orgDataServer)
        {
            if (orgDataServer != null)
            {
                ConnectionString = string.Format("Data Source={0};User ID={1};Password={2};Initial Catalog={3};Pooling=true;max pool size=512;{4};MultipleActiveResultSets=true;"//MultipleActiveResultSets=true;
                    , orgDataServer.DataServerName, orgDataServer.DataAccountName, orgDataServer.DataPassword, orgDataServer.DatabaseName, CommandTimeOut > 0 ? string.Format("connect timeout={0};", CommandTimeOut) : "");
            }
        }

        public int CommandTimeOut { get; set; } = 10;
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; } = "System.Data.SqlClient";
        public string DbType { get; set; }
    }
}