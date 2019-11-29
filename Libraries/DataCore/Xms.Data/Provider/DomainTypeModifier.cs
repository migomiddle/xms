using System;
using System.Linq.Expressions;

namespace Xms.Data.Provider
{
    /// <summary>
    /// expression中的实体类型转换
    /// </summary>
    public class DomainTypeModifier : ExpressionVisitor
    {
        private readonly Type _domainType;

        public DomainTypeModifier(Type domainType)
        {
            _domainType = domainType;
        }

        public Expression Modify(Expression expression)
        {
            if (expression is LambdaExpression)
            {
                var exp = expression as LambdaExpression;
                var param = Expression.Parameter(_domainType, "n");
                var funcType = typeof(Func<,>).MakeGenericType(_domainType, typeof(bool));
                var funcDelegate = Expression.Lambda(funcType, exp.Body, param);
                return Visit(funcDelegate);
            }
            return Visit(expression);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is MemberExpression)
            {
                return base.VisitMember(node);
            }
            if (node.Expression.Type.IsInterface)
            {
                var param = Expression.Parameter(_domainType, (node.Expression as ParameterExpression).Name);
                return Expression.MakeMemberAccess(param, node.Member);
            }
            return base.VisitMember(node);
        }
    }
}