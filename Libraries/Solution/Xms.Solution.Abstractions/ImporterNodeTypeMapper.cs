using System;
using System.Collections.Concurrent;

namespace Xms.Solution.Abstractions
{
    /// <summary>
    /// 解决方案组件导入服务映射
    /// </summary>
    public class ImporterNodeTypeMapper
    {
        private static readonly ConcurrentDictionary<string, Type> ImporterTypes = new ConcurrentDictionary<string, Type>();

        public static void Add(string nodeName, Type type)
        {
            ImporterTypes.TryAdd(nodeName, type);
        }

        public static Type Get(string nodeName)
        {
            if (ImporterTypes.TryGetValue(nodeName, out Type type))
            {
                return type;
            }
            return null;
        }
    }
}