using System;
using System.Collections.Generic;
using System.Linq;

namespace Xms.Infrastructure.Utility
{
    public static class CollectionExtensions
    {
        //public static bool NotEmpty<T>(this IEnumerable<T> source)
        //{
        //    return source != null && source.Any();
        //}

        //public static bool IsEmpty<T>(this IEnumerable<T> source)
        //{
        //    return source == null || !source.Any();
        //}

        public static bool NotEmpty<T>(this ICollection<T> source)
        {
            return source != null && source.Any();
        }

        public static bool IsEmpty<T>(this ICollection<T> source)
        {
            return (source == null || !source.Any());
        }

        /// <summary>
        /// 组合转换为字符串
        /// </summary>
        /// <param name="items"></param>
        /// <param name="separator">分隔符</param>
        /// <param name="wrapItem">项目包裹符</param>
        /// <returns></returns>
        public static string CollectionToString(this ICollection<object> items, string separator, string wrapItem = "")
        {
            string result = string.Empty;
            if (items != null)
            {
                if (wrapItem.IsNotEmpty())
                {
                    var newList = new List<object>();
                    foreach (var item in items)
                    {
                        if (item != null)
                        {
                            newList.Add(wrapItem + item.ToString() + wrapItem);
                        }
                    }
                    result = string.Join(separator, newList);
                }
                else
                {
                    result = string.Join(separator, items);
                }
            }
            return result;
        }

        public static string[] ArrayItemToString(this Guid[] items)
        {
            var ids = new string[] { };
            var i = 0;
            items.ToList().ForEach((n) =>
            {
                ids[i] = n.ToString();
            });
            return ids;
        }
    }
}