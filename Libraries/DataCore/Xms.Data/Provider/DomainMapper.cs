using PetaPoco;
using System;
using System.Reflection;

namespace Xms.Data.Provider
{
    /// <summary>
    /// 实体映射
    /// </summary>
    public class DomainMapper : IMapper
    {
        public static Type GetImplementType(Type interfaceType)
        {
            //if (interfaceType.IsInterface)
            //{
            //    return DomainMaps.Get(interfaceType);
            //}
            return interfaceType;
        }

        public ColumnInfo GetColumnInfo(PropertyInfo pocoProperty)
        {
            return ColumnInfo.FromProperty(pocoProperty);
        }

        public Func<object, object> GetFromDbConverter(PropertyInfo targetProperty, Type sourceType)
        {
            return null;
        }

        public TableInfo GetTableInfo(Type pocoType)
        {
            //if (pocoType.IsInterface)
            //{
            //    pocoType = DomainMaps.Get(pocoType);
            //}
            var t = TableInfo.FromPoco(pocoType);
            return t;
        }

        public Func<object, object> GetToDbConverter(PropertyInfo sourceProperty)
        {
            return null;
        }
    }
}