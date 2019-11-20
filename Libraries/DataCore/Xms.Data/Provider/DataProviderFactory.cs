using System;
using Xms.Core.Data;
using Xms.Data.Abstractions;

namespace Xms.Data.Provider
{
    public static class DataProviderFactory
    {
        /// <summary>
        /// 获取数据处理实例
        /// </summary>
        /// <returns></returns>
        public static IDataProvider<T> GetInstance<T>(DataProviderType provider, IDataProviderOptions options) where T : class
        {
            IDataProvider<T> _repository = null;
            switch (provider)
            {
                case DataProviderType.MSSQL:
                    _repository = new DataProvider<T>(options);
                    break;

                //case DataProviderType.MYSQL:
                //    _repository = new MySqlProvider<T>();
                //    break;

                default:
                    break;
            }
            if (null == _repository)
            {
                throw new Exception("no data provider is matched");
            }

            return _repository;
        }
    }

    public enum DataProviderType
    {
        MSSQL = 1
        , MYSQL = 2
    }
}