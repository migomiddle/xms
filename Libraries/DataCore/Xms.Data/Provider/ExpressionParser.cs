using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;

namespace Xms.Data.Provider
{
    /// <summary>
    /// Lambda表达式解析
    /// </summary>
    public class ExpressionParser : IExpressionParser
    {
        public List<QueryParameter> Arguments { get; } = new List<QueryParameter>();
        public string QueryText { get; private set; }
        private int _index = 0;

        private readonly Func<Type, string, string> _formatColumn = (entityType, name) => { return string.Format("[{0}].[{1}]", entityType.Name, name); };

        /// <summary>
        ///
        /// </summary>
        /// <param name="formatColumn">like [table].[column]</param>
        public ExpressionParser(Func<Type, string, string> formatColumn = null)
        {
            if (formatColumn != null)
            {
                _formatColumn = formatColumn;
            }
        }

        /// <summary>
        /// 解析lamdba，生成Sql语句
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public string ToSql(Expression expression)
        {
            var s = Resolve(expression);
            this.QueryText += (this.QueryText.IsNotEmpty() ? GetOperationName(ExpressionType.And) : string.Empty) + s;
            return s;
        }

        private string Resolve(Expression expression)
        {
            if (expression is LambdaExpression)
            {
                LambdaExpression lambda = expression as LambdaExpression;
                expression = lambda.Body;
                return Resolve(expression);
            }
            else if (expression is BinaryExpression)//解析二元运算符
            {
                BinaryExpression binary = expression as BinaryExpression;
                if (binary.Left is MemberExpression)
                {
                    object value = GetValue(binary.Right);
                    return ParseFunc(binary.Left, value, binary.NodeType);
                }
                else if (binary.Left is MethodCallExpression && (binary.Right is UnaryExpression || binary.Right is MemberExpression || binary.Right is ConstantExpression))
                {
                    object value = GetValue(binary.Right);
                    return ParseLinqToObject(binary.Left, value, binary.NodeType);
                }
                else if (binary.Left is UnaryExpression)
                {
                    object value = GetValue(binary.Right);
                    return ParseFunc(binary.Left, value, binary.NodeType);
                }
            }
            else if (expression is UnaryExpression)//解析一元运算符
            {
                UnaryExpression unary = expression as UnaryExpression;
                if (unary.Operand is MethodCallExpression)
                {
                    return ParseLinqToObject(unary.Operand, false);
                }
                else if (unary.Operand is MemberExpression)
                {
                    return ParseFunc(unary.Operand, false, ExpressionType.Equal);
                }
            }
            else if (expression is MethodCallExpression)//解析扩展方法
            {
                return ParseLinqToObject(expression, true);
            }
            else if (expression is MemberExpression)//解析属性
            {
                return ParseFunc(expression, true, ExpressionType.Equal);
            }
            var body = expression as BinaryExpression;
            if (body == null)
            {
                throw new Exception("can not translate expression: " + expression);
            }

            var operatorName = GetOperationName(body.NodeType);
            var Left = Resolve(body.Left);
            var Right = Resolve(body.Right);
            string result = string.Format("({0} {1} {2})", Left, operatorName, Right);
            return result;
        }

        /// <summary>
        /// 根据条件生成对应的sql查询操作符
        /// </summary>
        /// <param name="expressiontype"></param>
        /// <returns></returns>
        private string GetOperationName(ExpressionType expressiontype)
        {
            switch (expressiontype)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return " and ";

                case ExpressionType.Equal:
                    return " = ";

                case ExpressionType.GreaterThan:
                    return " > ";

                case ExpressionType.GreaterThanOrEqual:
                    return " >= ";

                case ExpressionType.LessThan:
                    return " < ";

                case ExpressionType.LessThanOrEqual:
                    return " <= ";

                case ExpressionType.NotEqual:
                    return " <> ";

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return " or ";

                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return " + ";

                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return " - ";

                case ExpressionType.Divide:
                    return " / ";

                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return " * ";

                default:
                    throw new Exception(string.Format("operator \"{0}\" is not supported", expressiontype));
            }
        }

        private string ParseFunc(Expression left, object value, ExpressionType expressiontype)
        {
            MemberExpression me = null;
            if (left is UnaryExpression)
            {
                me = (left as UnaryExpression).Operand as MemberExpression;
            }
            else
            {
                me = left as MemberExpression;
            }
            string name = me.Member.Name;
            string operatorName = GetOperationName(expressiontype);
            return BuildQuery(me.Expression.Type, name, operatorName, value?.ToString());
        }

        private string ParseLinqToObject(Expression expression, object value, ExpressionType? expressiontype = null)
        {
            var methodCall = expression as MethodCallExpression;
            var methodName = methodCall.Method.Name;
            switch (methodName)
            {
                case "Like":
                    return Like(methodCall);

                case "In":
                    return In(methodCall, true);

                case "NotIn":
                    return In(methodCall, false);

                case "IsNull":
                    return IsNull(methodCall, true);

                case "IsNotNull":
                    return IsNull(methodCall, false);

                case "Count":
                    return Len(methodCall, value, expressiontype.Value);

                case "LongCount":
                    return Len(methodCall, value, expressiontype.Value);

                default:
                    //return InvokeFunc(methodCall);
                    throw new Exception(string.Format("method \"{0}\" is not supported", methodName));
            }
        }

        private string SetArgument(string name, object value)
        {
            string temp = "@" + _index;
            _index += 1;
            this.Arguments.Add(new QueryParameter(temp, value));
            return temp;
        }

        private string BuildQuery(Type entityType, string name, string op, string value)
        {
            string paramName = SetArgument(name, value);
            var fieldName = _formatColumn(entityType, name);
            var i = fieldName.IndexOf("AS");
            string result = string.Format("({0} {1} {2})", i > 1 ? fieldName.Substring(0, i - 1) : fieldName, op, paramName);
            return result;
        }

        private string In(MethodCallExpression expression, object isTrue)
        {
            List<string> inParas = new List<string>();
            int i = 0;
            if (expression.Arguments[1] is LambdaExpression)
            {
                Resolve(expression.Arguments[1] as LambdaExpression);
            }
            else if (expression.Arguments[1] is NewArrayExpression)
            {
                var newArray = ((NewArrayExpression)expression.Arguments[1]).Expressions;
                foreach (var n in newArray)
                {
                    string name_para = "InParameter" + i;
                    object value = null;
                    if (n is MemberExpression)
                    {
                        value = GetMemberValue(n as MemberExpression);
                    }
                    else if (n is ConstantExpression)
                    {
                        value = ((ConstantExpression)n).Value;
                    }
                    else if (n is MethodCallExpression)
                    {
                        value = GetMemberValue(GetMemberExpression((MethodCallExpression)n));// (((MethodCallExpression)n).Object as MemberExpression);
                    }
                    string Key = SetArgument(name_para, value);
                    inParas.Add(Key);
                    i++;
                }
            }
            else if (expression.Arguments[1] is MemberExpression) // like userid in(IList<Guid> ids)
            {
                var m = (MemberExpression)expression.Arguments[1];
                object value = GetMemberValue(m);
                if (value != null)
                {
                    string name_para = "InParameter" + i;
                    string Key = SetArgument(name_para, value);
                    inParas.Add(Key);
                }
            }
            if (inParas.IsEmpty())
            {
                return string.Empty;
            }
            string name = GetMemberName(expression);
            MemberExpression me = GetMemberExpression(expression);
            string operatorName = Convert.ToBoolean(isTrue) ? "IN" : "NOT IN";
            string paraName = string.Join(",", inParas);
            string result = string.Format("{0} {1} ({2})", _formatColumn(me.Expression.Type, name), operatorName, paraName);
            return result;
        }

        private string Like(MethodCallExpression expression)
        {
            object val = GetValue(expression.Arguments[1]);
            string value = string.Format("%{0}%", val);
            string name = string.Empty;
            MemberExpression me = GetMemberExpression(expression);
            name = GetMemberName(expression);
            string paraName = SetArgument(name, value);
            string result = string.Format("{0} LIKE {1}", _formatColumn(me.Expression.Type, name), paraName);
            return result;
        }

        private string Len(MethodCallExpression expression, object value, ExpressionType expressiontype)
        {
            string name = string.Empty;
            MemberExpression me = GetMemberExpression(expression);
            name = GetMemberName(expression);
            string operatorName = GetOperationName(expressiontype);
            string paraName = SetArgument(name, value.ToString());
            string result = string.Format("LEN({0}){1}{2}", _formatColumn(me.Expression.Type, name), operatorName, paraName);
            return result;
        }

        private string IsNull(MethodCallExpression expression, bool isNull = true)
        {
            string name = string.Empty;
            MemberExpression me = GetMemberExpression(expression);
            name = GetMemberName(expression);
            string result = string.Format("{0} IS{1}NULL", _formatColumn(me.Expression.Type, name), isNull ? " " : " NOT ");
            return result;
        }

        private string GetMemberName(MethodCallExpression expression)
        {
            string name = string.Empty;
            if (expression.Arguments[0] is UnaryExpression)
            {
                name = (((UnaryExpression)expression.Arguments[0]).Operand as MemberExpression).Member.Name;
            }
            else
            {
                name = ((MemberExpression)expression.Arguments[0]).Member.Name;
            }
            return name;
        }

        private MemberExpression GetMemberExpression(MethodCallExpression expression)
        {
            MemberExpression me = null;

            if (expression.Arguments[0] is MemberExpression)
            {
                me = expression.Arguments[0] as MemberExpression;
            }
            else if (expression.Arguments[0] is UnaryExpression)
            {
                me = ((UnaryExpression)expression.Arguments[0]).Operand as MemberExpression;
            }
            else if (expression.Object is MemberExpression)
            {
                me = expression.Object as MemberExpression;
            }

            return me;
        }

        // 获取属性值
        private object GetValue(Expression expression)
        {
            if (expression is ConstantExpression)
            {
                return (expression as ConstantExpression).Value;
            }
            else if (expression is UnaryExpression)
            {
                UnaryExpression unary = expression as UnaryExpression;
                if (unary.Operand is MemberExpression)
                {
                    return GetMemberValue(unary.Operand as MemberExpression);
                }
                else
                {
                    LambdaExpression lambda = Expression.Lambda(unary.Operand);
                    Delegate fn = lambda.Compile();
                    return fn.DynamicInvoke(null);
                }
            }
            else if (expression is MemberExpression)
            {
                return GetMemberValue(expression as MemberExpression);
            }
            else if (expression is MethodCallExpression)
            {
                var expCall = (expression as MethodCallExpression);
                LambdaExpression exp = Expression.Lambda(expCall);
                return exp.Compile().DynamicInvoke(null);
            }
            throw new Exception("can not get value from " + expression);
        }

        // 获取属性值
        private object GetMemberValue(MemberExpression memberExpression)
        {
            MemberInfo memberInfo;
            object obj = null;
            object result = null;

            if (memberExpression == null)
            {
                throw new ArgumentNullException("memberExpression");
            }

            if (memberExpression.Expression is ConstantExpression)
            {
                obj = ((ConstantExpression)memberExpression.Expression).Value;
            }
            else if (memberExpression.Expression is MemberExpression)
            {
                obj = GetMemberValue((MemberExpression)memberExpression.Expression);
            }
            //else
            //{
            //    throw new NotSupportedException("not supported expression type: "
            //        + memberExpression.Expression.GetType().FullName);
            //}

            memberInfo = memberExpression.Member;
            if (memberInfo is PropertyInfo)
            {
                PropertyInfo property = (PropertyInfo)memberInfo;
                result = property.GetValue(obj, null);
                Type type = property.PropertyType;
                if (type.IsEnum)
                {
                    result = (int)result;
                }
            }
            else if (memberInfo is FieldInfo)
            {
                FieldInfo field = (FieldInfo)memberInfo;
                result = field.GetValue(obj);
                Type type = field.FieldType;
                if (type.IsEnum)
                {
                    result = (int)result;
                }
            }
            else
            {
                throw new NotSupportedException("not supported member: "
                    + memberInfo.GetType().FullName);
            }
            return result;
        }
    }
}