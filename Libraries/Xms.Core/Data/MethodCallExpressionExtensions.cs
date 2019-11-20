using System;
using System.Collections.Generic;

namespace Xms.Core.Data
{
    /// <summary>
    /// 表达式扩展方法
    /// </summary>
    public static class MethodCallExpressionExtensions
    {
        public const string Tips = "此扩展方法只能用于框架内部";

        /// <summary>
        /// IN
        /// </summary>
        public static bool In(this object member, IEnumerable<object> values)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// IN
        /// </summary>
        public static bool In(this object member, params object[] values)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// NOT IN
        /// </summary>
        public static bool NotIn(this object member, IEnumerable<object> values)
        {
            throw new Exception(Tips);
        }

        public static bool NotIn(this object member, params object[] values)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// LIKE
        /// </summary>
        public static bool Like(this string member, string values)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// IS NULL
        /// </summary>
        public static bool IsNull(this object member)
        {
            throw new Exception(Tips);
        }

        /// <summary>
        /// IS NOT NULL
        /// </summary>
        public static bool IsNotNull(this object member)
        {
            throw new Exception(Tips);
        }
    }
}