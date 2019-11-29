using PetaPoco;
using PetaPoco.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xms.Core;
using Xms.Core.Context;
using Xms.Infrastructure.Utility;

namespace Xms.Data.Provider
{
    /// <summary>
    /// petapoco框架助手
    /// </summary>
    public class PocoHelper
    {
        /// <summary>
        /// 生成查询语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco"></param>
        /// <param name="q"></param>
        /// <param name="otherCondition"></param>
        /// <param name="isCount"></param>
        /// <returns></returns>
        public static Sql ParseSelectSql<T>(PocoData poco, QueryDescriptor<T> q, Sql otherCondition = null, bool isCount = false) where T : class
        {
            var columns = GetSelectColumns(poco, q?.Columns, out List<string> froms, isCount);
            Sql query = Sql.Builder.Append("SELECT " + columns);
            //from，由select中反推
            query.From(string.Join("\n", froms));

            //过滤条件
            query.Append(GetConditions(q, otherCondition));

            //排序
            if (isCount == false)
            {
                query.Append(GetOrderBy(poco, q?.SortingDescriptor));
            }

            return query;
        }

        /// <summary>
        /// 格式化列名为：[表名].[列名]
        /// </summary>
        /// <param name="poco"></param>
        /// <param name="name"></param>
        /// <param name="fromTable"></param>
        /// <returns></returns>
        public static string FormatColumn(PocoData poco, string name)
        {
            return FormatColumn(poco, name, out string _);
        }

        /// <summary>
        /// 格式化列名为：[表名].[列名]
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="name"></param>
        /// <param name="fromTable"></param>
        /// <returns></returns>
        public static string FormatColumn(Type entityType, string name)
        {
            //entitytype这里要替换为impletation type
            entityType = DomainMapper.GetImplementType(entityType);
            return FormatColumn(entityType, name, out string _);
        }

        /// <summary>
        /// 格式化列名为：[表名].[列名]
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="name"></param>
        /// <param name="fromTable"></param>
        /// <returns></returns>
        public static string FormatColumn(Type entityType, string name, out string fromTable)
        {
            //entitytype这里要替换为impletation type
            entityType = DomainMapper.GetImplementType(entityType);
            return FormatColumn(PocoData.ForType(entityType, new DomainMapper()), name, out fromTable);
        }

        /// <summary>
        /// 格式化列名为：[表名].[列名]
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="name"></param>
        /// <param name="fromTable"></param>
        /// <returns></returns>
        public static string FormatColumn(PocoData poco, string name, out string fromTable)
        {
            var column = poco.Columns.First(n => n.Key.IsCaseInsensitiveEqual(name));
            string result = name;
            var p = column.Value.PropertyInfo;
            var attrs = p.GetCustomAttribute(typeof(LinkEntityAttribute), true);
            if (attrs != null)
            {
                var leAttr = (LinkEntityAttribute)attrs;
                string linkTableName = leAttr.LinkToTableName;
                if (linkTableName.IsEmpty())
                {
                    var linkTarget = leAttr.Target;
                    var linkPoco = new PocoData(linkTarget, new DomainMapper());
                    linkTableName = linkPoco.TableInfo.TableName;
                }
                leAttr.AliasName = leAttr.AliasName.IfEmpty(linkTableName);
                var fieldName = leAttr.TargetFieldName.IfEmpty(name);
                result = "{0}.{1}".FormatWith(WrapName(leAttr.AliasName), WrapName(fieldName));// "[" + leAttr.AliasName + "].[" + fieldName + "]";// AS " + name;//关联表字段
                //如果LinkFromFieldName、LinkToFieldName为空，则查找实体中是否存在字段与被连接实体的主键名称匹配
                if (leAttr.LinkFromFieldName.IsEmpty())
                {
                    if (poco.Columns.Count(n => n.Key.IsCaseInsensitiveEqual(linkTableName + "id")) > 0)
                    {
                        var linkField = poco.Columns.First(n => n.Key.IsCaseInsensitiveEqual(linkTableName + "id"));
                        leAttr.LinkFromFieldName = linkField.Key;
                        leAttr.LinkToFieldName = linkTableName + "id";
                    }
                }
                string joinCondition = "{0}.{1}={2}.{3}".FormatWith(leAttr.AliasName, WrapName(leAttr.LinkToFieldName.IfEmpty(name)), poco.TableInfo.TableName, WrapName(leAttr.LinkFromFieldName.IfEmpty(name)));
                fromTable = "LEFT JOIN {0} AS {1} WITH(NOLOCK) ON {2}".FormatWith(WrapName(linkTableName), WrapName(leAttr.AliasName), joinCondition);
            }
            else
            {
                result = "{0}.{1}".FormatWith(WrapName(poco.TableInfo.TableName), WrapName(name));//主表字段
                fromTable = poco.TableInfo.TableName + " WITH(NOLOCK)";
            }
            return result;
        }

        /// <summary>
        /// 获取查询列名
        /// </summary>
        /// <param name="poco"></param>
        /// <param name="columns"></param>
        /// <param name="isCount"></param>
        /// <returns></returns>
        public static string GetSelectColumns(PocoData poco, List<string> columns, out List<string> froms, bool isCount = false)
        {
            froms = new List<string>();
            froms.Add(poco.TableInfo.TableName + " WITH(NOLOCK)");
            if (columns == null)
            {
                columns = new List<string>();
            }
            if (isCount)
            {
                columns.Add("COUNT(1)");
            }
            else
            {
                if (columns.Count == 0)
                {
                    //添加所有列
                    foreach (var item in poco.Columns)
                    {
                        var c = item.Value;
                        string name = item.Key;
                        if (name.IsNotEmpty())
                        {
                            columns.Add(name);
                        }
                    }
                }
                else if (!columns.Contains(poco.TableInfo.PrimaryKey))
                {
                    //添加主键列
                    columns.Add(poco.TableInfo.PrimaryKey);
                }
                //格式化列名
                for (int i = 0; i < columns.Count; i++)
                {
                    var item = columns[i];
                    var itemIndex = columns.IndexOf(item);
                    var fromTable = string.Empty;
                    columns[itemIndex] = FormatColumn(poco, item, out fromTable) + " AS " + WrapName(item);
                    if (!froms.Contains(fromTable))
                    {
                        froms.Add(fromTable);
                    }
                }
            }
            return string.Join(",", columns);
        }

        /// <summary>
        /// 获取过滤条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <param name="otherCondition"></param>
        /// <returns></returns>
        public static Sql GetConditions<T>(QueryDescriptor<T> q, Sql otherCondition = null) where T : class
        {
            if (q != null && q.QueryText.IsNotEmpty())
            {
                return GetConditions(q.QueryText, q.Parameters, otherCondition);
            }
            return Sql.Builder;
        }

        /// <summary>
        /// 获取过滤条件
        /// </summary>
        /// <param name="queryText"></param>
        /// <param name="p"></param>
        /// <param name="otherCondition"></param>
        /// <returns></returns>
        public static Sql GetConditions(string queryText, List<QueryParameter> p, Sql otherCondition = null)
        {
            //过滤条件
            Sql filter = Sql.Builder;
            if (queryText.IsNotEmpty() && p.NotEmpty())
            {
                var values = p.Select(n => n.Value).ToArray();
                //AnsiString
                int i = 0;
                foreach (var item in values)
                {
                    if (item is string)
                    {
                        values[i] = new AnsiString(item.ToString());
                    }
                    i++;
                }
                filter.Append("WHERE");
                filter.Append(queryText, values);
            }
            if (otherCondition != null && otherCondition.SQL.IsNotEmpty())
            {
                if (filter.SQL.IsEmpty())
                {
                    filter.Append("WHERE");
                }
                filter.Append(otherCondition);
            }
            return filter;
        }

        /// <summary>
        /// 获取排序语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco"></param>
        /// <param name="sortingDescriptor"></param>
        /// <returns></returns>
        public static Sql GetOrderBy<T>(PocoData poco, List<SortDescriptor<T>> sortingDescriptor) where T : class
        {
            Sql query = Sql.Builder;
            //排序
            if (sortingDescriptor != null && sortingDescriptor.Count > 0)
            {
                List<string> ord = new List<string>();
                foreach (var item in sortingDescriptor)
                {
                    if (item.Field.IsNotEmpty())
                    {
                        ord.Add("{0}.{1} {2}".FormatWith(WrapName(poco.TableInfo.TableName), WrapName(item.Field), item.GetDbDirectionName()));
                    }
                }
                if (ord.Count > 0)
                {
                    query.Append(" ORDER BY ");
                    query.Append(string.Join(",", ord));
                }
            }

            return query;
        }

        /// <summary>
        /// 转换为数据库执行上下文
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco"></param>
        /// <param name="q"></param>
        /// <param name="otherCondition"></param>
        /// <param name="isCount"></param>
        /// <returns></returns>
        public static ExecuteContext<T> ParseContext<T>(PocoData poco, QueryDescriptor<T> q, Sql otherCondition = null, bool isCount = false) where T : class
        {
            Sql s = ParseSelectSql<T>(poco, q, otherCondition, isCount);
            ExecuteContext<T> ctx = new ExecuteContext<T>()
            {
                ExecuteContainer = s
                ,
                PagingInfo = q?.PagingDescriptor
                ,
                TopCount = q == null ? 0 : q.TopCount
            };

            return ctx;
        }

        /// <summary>
        /// 转换为数据库执行上下文
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <param name="poco"></param>
        /// <param name="q"></param>
        /// <param name="otherCondition"></param>
        /// <param name="isCount"></param>
        /// <returns></returns>
        public static ExecuteContext<TImpl> ParseContext<T, TImpl>(PocoData poco, QueryDescriptor<T> q, Sql otherCondition = null, bool isCount = false)
            where T : class
            where TImpl : class, T
        {
            Sql s = ParseSelectSql<T>(poco, q, otherCondition, isCount);
            ExecuteContext<TImpl> ctx = new ExecuteContext<TImpl>()
            {
                ExecuteContainer = s
                ,
                PagingInfo = q?.PagingDescriptor
                ,
                TopCount = q == null ? 0 : q.TopCount
            };

            return ctx;
        }

        /// <summary>
        /// 转换为数据库执行上下文
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco"></param>
        /// <param name="q"></param>
        /// <param name="otherCondition"></param>
        /// <param name="isCount"></param>
        /// <returns></returns>
        public static ExecuteContext<T> ParseContext<T>(QueryDescriptor<T> q, Sql otherCondition = null, bool isCount = false) where T : class
        {
            Sql s = ParseSelectSql(PocoData.ForType(typeof(T), new DomainMapper()), q, otherCondition, isCount);
            ExecuteContext<T> ctx = new ExecuteContext<T>()
            {
                ExecuteContainer = s
                ,
                PagingInfo = q?.PagingDescriptor
                ,
                TopCount = q == null ? 0 : q.TopCount
            };

            return ctx;
        }

        /// <summary>
        /// 生成更新语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Sql ParseUpdateSql<T>(PocoData poco, UpdateContext<T> q) where T : class
        {
            Sql query = Sql.Builder.Append("UPDATE {0} SET ".FormatWith(WrapName(poco.TableInfo.TableName)));
            int index = 0;
            foreach (var item in q.Sets)
            {
                query.Append((index > 0 ? "," : "") + FormatColumn(poco, item.Key) + "=@0", item.Value);
                index++;
            }
            if (q != null && q.QueryText.IsNotEmpty())
            {
                query.Append(GetConditions(q.QueryText, q.Parameters));
            }
            return query;
        }

        public static string WrapName(string name)
        {
            var dbType = "mssql";// XmsApplication.Configuration["database:dbtype"];
            return dbType.IsCaseInsensitiveEqual("mssql") ? "[{0}]".FormatWith(name) : name;
        }
    }
}