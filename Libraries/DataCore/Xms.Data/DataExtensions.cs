using System;
using System.Data;

namespace Xms.Data
{
    public static class DataExtensions
    {
        public static DataSet ExecuteQuery(this DbContext context, string sql, params object[] args)
        {
            DataSet result = new DataSet();
            try
            {
                context.OpenSharedConnection();

                try
                {
                    using (var cmd = context.CreateCommand(context.Connection, sql, args))
                    {
                        //using (SqlDataAdapter dbDataAdapter = new SqlDataAdapter((SqlCommand)cmd))
                        //{
                        //    dbDataAdapter.Fill(result);
                        //}
                    }
                }
                finally
                {
                    context.CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                context.OnException(x);
                throw;
            }
            return result;
        }
    }
}