using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Xms.Infrastructure.Utility
{
    public class ExpressionHelper
    {
        //获取类成员名称
        public static string GetPropertyName<T>(Expression<Func<T, object>> e)
        {
            return ExpressionRouter(e);
        }

        public static string GetPropertyName<T, TPropType>(Expression<Func<T, TPropType>> e)
        {
            return ExpressionRouter(e);
        }

        //获取类成员名称
        public static List<string> GetPropertyNames<T>(params Expression<Func<T, object>>[] fields)
        {
            var columns = new List<string>();
            foreach (var item in fields)
            {
                if (item.Body is NewExpression)
                {
                    var m = (item.Body as NewExpression).Members;
                    columns.AddRange(m.AsEnumerable().Select(f => f.Name));
                }
                else if (item.Body is MemberExpression)
                {
                    columns.Add((item.Body as MemberExpression).Member.Name);
                }
                else if (item.Body is UnaryExpression)
                {
                    var me = (item.Body as UnaryExpression).Operand as MemberExpression;
                    columns.Add(me.Member.Name);
                }
            }
            return columns;
        }

        private static string ExpressionRouter(Expression exp)
        {
            string sb = string.Empty;
            if (exp is LambdaExpression)
            {
                LambdaExpression le = ((LambdaExpression)exp);
                return LambdaExpressionProvider(le);
            }
            else if (exp is BinaryExpression)
            {
                BinaryExpression be = ((BinaryExpression)exp);
                return BinaryExpressionProvider(be);
            }
            else if (exp is MemberExpression)
            {
                MemberExpression me = ((MemberExpression)exp);
                return MemberExpressionProvider(me);
            }
            else if (exp is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp);
                return UnaryExpressionProvider(ue);
            }
            return null;
        }

        private static string LambdaExpressionProvider(LambdaExpression le)
        {
            return ExpressionRouter(le.Body);
        }

        private static string BinaryExpressionProvider(BinaryExpression be)
        {
            return ExpressionRouter(be.Left);
        }

        private static string MemberExpressionProvider(MemberExpression me)
        {
            return me.Member.Name;
        }

        private static string UnaryExpressionProvider(UnaryExpression ue)
        {
            return ExpressionRouter(ue.Operand);
        }
    }
}