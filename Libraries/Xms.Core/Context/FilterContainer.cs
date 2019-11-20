using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Xms.Core.Data;

namespace Xms.Core.Context
{
    /// <summary>
    /// 过滤条件对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilterContainer<T> where T : class
    {
        private readonly StringBuilder _queryText = new StringBuilder();
        private readonly IExpressionParser _expressionParser;

        public List<QueryParameter> Parameters { get; private set; } = new List<QueryParameter>();

        public string QueryText
        {
            get
            {
                return _queryText.ToString();
            }
        }

        public FilterContainer(IExpressionParser expressionParser)
        {
            _expressionParser = expressionParser;
        }

        public FilterContainer<T> And(Expression<Func<T, bool>> predicate)
        {
            if (_queryText.Length > 0)
            {
                _queryText.Append(" AND ");
            }
            _queryText.Append(_expressionParser.ToSql(predicate));
            Parameters = _expressionParser.Arguments;
            return this;
        }

        public FilterContainer<T> Or(Expression<Func<T, bool>> predicate)
        {
            if (_queryText.Length > 0)
            {
                _queryText.Append(" OR ");
            }
            _queryText.Append(_expressionParser.ToSql(predicate));
            Parameters = _expressionParser.Arguments;
            return this;
        }
    }
}