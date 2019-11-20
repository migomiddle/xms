using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;

namespace Xms.Core.Data
{
    public interface IExpressionParser
    {
        List<QueryParameter> Arguments { get; }
        string QueryText { get; }

        string ToSql(Expression expression);
    }
}