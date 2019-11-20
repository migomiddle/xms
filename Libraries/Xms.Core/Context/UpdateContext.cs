using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;

namespace Xms.Core.Context
{
    /// <summary>
    /// 更新上下文
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpdateContext<T> where T : class
    {
        private IExpressionParser _expressionParser;
        public string QueryText { get; private set; } = string.Empty;
        public List<QueryParameter> Parameters { get; private set; } = new List<QueryParameter>();

        public List<KeyValuePair<string, object>> Sets { get; } = new List<KeyValuePair<string, object>>();

        public UpdateContext(IExpressionParser expressionParser)
        {
            _expressionParser = expressionParser;
        }

        public UpdateContext<T> Where(Expression<Func<T, bool>> predicate)
        {
            _expressionParser.ToSql(predicate);
            this.QueryText = _expressionParser.QueryText;
            this.Parameters = _expressionParser.Arguments;
            return this;
        }

        public UpdateContext<T> Set(Expression<Func<T, object>> fieldPath, object value)
        {
            var field = ExpressionHelper.GetPropertyName<T>(fieldPath);
            Sets.Add(new KeyValuePair<string, object>(field, value));
            return this;
        }
    }

    public static class UpdateExtensions
    {
        public const string Tips = "此方法只能用于本框架";

        public static void Val(this object member, object value)
        {
            throw new Exception(Tips);
        }
    }
}