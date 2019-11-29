using System.Data;
using System.Data.SqlClient;

namespace Xms.Infrastructure.Utility
{
    public class DataBaseHelper
    {
        public static string GetDbConfiguration(string DataServerName, string DataAccountName, string DataPassword, string DatabaseName, int CommandTimeOut)
        {
            return string.Format("Data Source={0};User ID={1};Password={2};Initial Catalog={3};Pooling=true;max pool size=512;{4};MultipleActiveResultSets=true;"//MultipleActiveResultSets=true;
                      , DataServerName, DataAccountName, DataPassword, DatabaseName, CommandTimeOut > 0 ? string.Format("connect timeout={0};", CommandTimeOut) : "");
        }

        /// <summary>
        /// 测试连接数据库是否成功
        /// </summary>
        /// <returns></returns>
        public static bool ConnectionTest(string connectionString)
        {
            bool isCanConnectioned = false;
            SqlConnection mySqlConnection = new SqlConnection(connectionString);
            try
            {
                mySqlConnection.Open();
                isCanConnectioned = true;
            }
            catch
            {
                isCanConnectioned = false;
            }
            finally
            {
                mySqlConnection.Close();
            }
            if (mySqlConnection.State == ConnectionState.Closed || mySqlConnection.State == ConnectionState.Broken)
            {
                return isCanConnectioned;
            }
            else
            {
                return isCanConnectioned;
            }
        }
    }
}