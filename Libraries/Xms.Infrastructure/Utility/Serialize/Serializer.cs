using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Xms.Infrastructure.Utility.Serialize
{
    public static class Serializer
    {
        /// <summary>
        /// XML序列化
        /// </summary>
        /// <param name="source">序列对象</param>
        /// <param name="filePath">XML文件路径</param>
        /// <returns>是否成功</returns>
        public static bool ToXmlFile<T>(T entity, string filePath)
        {
            bool result = false;

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                XmlSerializer serializer = new XmlSerializer(entity.GetType());
                serializer.Serialize(fs, entity);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// XML序列化
        /// </summary>
        /// <param name="source">序列对象</param>
        /// <returns></returns>
        public static string ToXml<T>(T entity)
        {
            StringWriter sw = new StringWriter();
            try
            {
                XmlSerializer serializer = new XmlSerializer(entity.GetType());
                serializer.Serialize(sw, entity);
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                sw.Dispose();
            }
            return sw.ToString();
        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <param name="entity">目标对象</param>
        /// <param name="filePath">XML文件路径</param>
        /// <returns>序列对象</returns>
        public static T FromXmlFile<T>(T entity, string filePath, XmlAttributeOverrides xOver = null) where T : class
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                XmlSerializer serializer = xOver == null ? new XmlSerializer(typeof(T)) : new XmlSerializer(typeof(T), xOver);
                entity = serializer.Deserialize(fs) as T;
                return entity;
            }
        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">目标对象</param>
        /// <param name="xml"></param>
        /// <param name="xOver"></param>
        /// <returns></returns>
        public static T FromXml<T>(T entity, string xml, XmlAttributeOverrides xOver = null) where T : class
        {
            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer serializer = xOver == null ? new XmlSerializer(typeof(T)) : new XmlSerializer(typeof(T), xOver);
                entity = serializer.Deserialize(sr) as T;
                return entity;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static object FromXml(Type type, string xml, XmlAttributeOverrides xOver = null)
        {
            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer serializer = xOver == null ? new XmlSerializer(type) : new XmlSerializer(type, xOver);
                return serializer.Deserialize(sr);
            }
        }

        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="obj">序列对象</param>
        /// <returns>是否成功</returns>
        public static string ToJson<T>(T source, bool nameLower = true, List<string> excludedProperties = null)
        {
            JsonSerializerSettings jss = new JsonSerializerSettings();
            if (excludedProperties != null && excludedProperties.Count > 0)
            {
                jss.ContractResolver = new JsonPropertyContractResolver(excludedProperties, nameLower);
            }
            else if (nameLower)
            {
                jss.ContractResolver = new JsonLowercaseContractResolver();
            }
            jss.MaxDepth = 10;
            jss.NullValueHandling = NullValueHandling.Ignore;
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            jss.Converters.Add(timeFormat);
            //jss.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            //jss.Converters.Add(new BoolConvert("是,否"));
            return JsonConvert.SerializeObject(source, Formatting.None, jss);
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <param name="entity">目标对象</param>
        /// <param name="data">json</param>
        /// <returns>序列对象</returns>
        public static T FromJson<T>(T entity, string data) where T : class
        {
            JsonSerializerSettings jss = new JsonSerializerSettings();

            jss.NullValueHandling = NullValueHandling.Ignore;
            //jss.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            //jss.Converters.Add(new BoolConvert("是,否"));

            return JsonConvert.DeserializeObject<T>(data, jss);
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="data">json</param>
        /// <returns></returns>
        public static object FromJson(Type type, string data)
        {
            JsonSerializerSettings jss = new JsonSerializerSettings();

            jss.NullValueHandling = NullValueHandling.Ignore;

            return JsonConvert.DeserializeObject(data, type, jss);
        }
    }
}