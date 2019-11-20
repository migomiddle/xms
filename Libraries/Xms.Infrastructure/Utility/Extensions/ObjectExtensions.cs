using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Xms.Infrastructure.Utility.Serialize;

namespace Xms.Infrastructure.Utility
{
    public static class ObjectExtensions
    {
        #region 序列化

        /// <summary>
        /// XML序列化
        /// </summary>
        /// <param name="source">序列对象</param>
        /// <param name="filePath">XML文件路径</param>
        /// <returns>是否成功</returns>
        public static bool SerializeToXml(this object source, string filePath)
        {
            return Serializer.ToXmlFile(source, filePath);
        }

        /// <summary>
        /// XML序列化
        /// </summary>
        /// <param name="source">序列对象</param>
        /// <returns></returns>
        public static string SerializeToXml(this object source)
        {
            return Serializer.ToXml(source);
        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="filePath">XML文件路径</param>
        /// <returns>序列对象</returns>
        public static T DeserializeFromXMLFile<T>(this T target, string filePath, XmlAttributeOverrides xOver = null) where T : class
        {
            return Serializer.FromXmlFile(target, filePath);
        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="xOver"></param>
        /// <returns></returns>
        public static T DeserializeFromXMLString<T>(this T target, string source, XmlAttributeOverrides xOver = null) where T : class
        {
            return Serializer.FromXml(target, source);
        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object DeserializeFromXMLString(this Type type, string data)
        {
            return Serializer.FromXml(type, data);
        }

        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="obj">序列对象</param>
        /// <returns>是否成功</returns>
        public static string SerializeToJson(this object source, bool nameLower = true, List<string> excludedProperties = null)
        {
            return Serializer.ToJson(source, nameLower, excludedProperties);
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="data">json</param>
        /// <returns>序列对象</returns>
        public static T DeserializeFromJson<T>(this T target, string data) where T : class
        {
            return Serializer.FromJson(target, data);
        }

        /// <summary>
        /// JSON
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object DeserializeFromJson(this Type type, string data)
        {
            return Serializer.FromJson(type, data);
        }

        #endregion 序列化

        /// <summary>
        /// 对象间拷贝
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="sourceInstance"></param>
        /// <param name="targetInstance"></param>
        public static void CopyTo<TSource, TDest>(this TSource sourceInstance, TDest targetInstance)
            where TSource : class
            where TDest : class
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TDest);
            var targetProps = targetType.GetProperties().ToList();
            foreach (var item in targetProps)
            {
                if (item.CanWrite)
                {
                    var sourceProp = sourceType.GetProperty(item.Name);
                    if (null != sourceProp && sourceProp.CanRead)
                    {
                        targetType.SetPropertyValue(targetInstance, item.Name, sourceProp.GetValue(sourceInstance));
                    }
                }
            }
        }

        public static object ChangeType(this object value, Type type)
        {
            if (value == null && type.IsGenericType)
            {
                return Activator.CreateInstance(type);
            }

            if (value == null)
            {
                return null;
            }

            if (type == value.GetType())
            {
                return value;
            }

            if (type.IsEnum)
            {
                if (value is string)
                {
                    return Enum.Parse(type, value as string);
                }
                else
                {
                    return Enum.ToObject(type, value);
                }
            }
            if (!type.IsInterface && type.IsGenericType)
            {
                Type innerType = type.GetGenericArguments()[0];
                object innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(type, new object[] { innerValue });
            }
            if (value is string && type == typeof(Guid))
            {
                return new Guid(value as string);
            }

            if (value is string && type == typeof(Version))
            {
                return new Version(value as string);
            }

            if (!(value is IConvertible))
            {
                return value;
            }

            return Convert.ChangeType(value, type);
        }

        public static bool IsEmpty(this Guid value)
        {
            return value.Equals(Guid.Empty);
        }
    }
}