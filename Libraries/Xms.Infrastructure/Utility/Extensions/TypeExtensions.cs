using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Xms.Infrastructure.Utility
{
    public static class TypeExtensions
    {
        public static bool HasAttribute<TAttribute>(this ICustomAttributeProvider target, bool inherits) where TAttribute : Attribute
        {
            return target.IsDefined(typeof(TAttribute), inherits);
        }

        /// <summary>
        /// 对象间拷贝
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="sourceType"></param>
        /// <param name="sourceInstance"></param>
        /// <param name="targetInstance"></param>
        //public static void CopyTo<TSource, TDest>(this Type sourceType, TSource sourceInstance, TDest targetInstance)
        //    where TSource : class , new()
        //    where TDest : class, new()
        //{
        //    var targetType = typeof(TDest);
        //    var targetProps = targetType.GetProperties().ToList();
        //    foreach (var item in targetProps)
        //    {
        //        if (item.CanWrite)
        //        {
        //            var sourceProp = sourceType.GetProperty(item.Name);
        //            if (null != sourceProp && sourceProp.CanRead)
        //            {
        //                targetType.SetPropertyValue(targetInstance, item.Name, sourceProp.GetValue(sourceInstance));
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// 设置对象的属性值
        /// </summary>
        /// <param name="target"></param>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetPropertyValue(this Type target, object instance, string name, object value)
        {
            PropertyInfo p = target.GetProperty(name);
            object v = p.PropertyType.GetFieldValue(value);
            if (p != null && p.CanWrite)
            {
                p.SetValue(instance, v);
                return true;
            }
            return false;
        }

        public static object GetFieldValue(this Type propType, object fieldValue)
        {
            if (Convert.IsDBNull(fieldValue) || fieldValue == null)
            {
                return null;
            }
            else
            {
                if (propType.IsGenericType && propType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    if (fieldValue != null)
                    {
                        NullableConverter nullableConverter = new NullableConverter(propType);
                        propType = nullableConverter.UnderlyingType;
                    }
                    else
                    {
                        return propType.TypeInitializer;
                    }
                }

                return Convert.ChangeType(fieldValue, propType);
            }
        }

        /// <summary>
        /// Gets the underlying type of a <see cref="Nullable{T}" /> type.
        /// </summary>
        public static Type GetNonNullableType(this Type type)
        {
            if (!IsNullable(type))
            {
                return type;
            }
            return type.GetGenericArguments()[0];
        }

        public static bool IsInteger(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsNullable(this Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsCollection(this Type type)
        {
            return type.GetInterfaces().Any(n => n.Name == "IEnumerable");
        }

        public static bool IsValueType(this Type type)
        {
            var result = IsInteger(type);
            if (!result)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.String:
                    case TypeCode.Boolean:
                    case TypeCode.Char:
                    case TypeCode.DateTime:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Empty:
                    case TypeCode.Single:
                        result = true;
                        break;

                    default:
                        result = false;
                        break;
                }
            }
            return result;
        }
    }
}