namespace Xms.Data.Abstractions
{
    public interface IDataProviderOptions
    {
        string DbType { get; set; }
        string ProviderName { get; set; }
        string ConnectionString { get; set; }
        int CommandTimeOut { get; set; }
    }
}