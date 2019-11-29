using System;
using System.Collections.Generic;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 数据查询扩展方法
    /// </summary>
    public static class DataFinderExtensions
    {
        public static Entity RetrieveById(this IDataFinder finder, string entityName, Guid id, string primarykey = "", bool ignorePermissions = false)
        {
            var q = new QueryExpression(entityName, LanguageCode.CHS);
            q.ColumnSet.AllColumns = true;
            q.Criteria.AddCondition(primarykey.IfEmpty(entityName + "ID"), ConditionOperator.Equal, id);

            return finder.Retrieve(q, ignorePermissions);
        }

        public static List<Entity> RetrieveAll(this IDataFinder finder, string entityName, List<string> columns = null, OrderExpression order = null, bool ignorePermissions = false)
        {
            var q = new QueryExpression(entityName, LanguageCode.CHS);
            if (columns.NotEmpty())
            {
                q.ColumnSet.AddColumns(columns.ToArray());
            }
            else
            {
                q.ColumnSet.AllColumns = true;
            }
            if (order != null)
            {
                q.AddOrder(order.AttributeName, order.OrderType);
            }

            return finder.RetrieveAll(q, ignorePermissions);
        }

        public static Entity RetrieveByAttribute(this IDataFinder finder, string entityName, Dictionary<string, object> key_value, List<string> columns = null, bool ignorePermissions = false)
        {
            var q = new QueryExpression(entityName, LanguageCode.CHS);
            if (columns.IsEmpty())
            {
                q.ColumnSet.AllColumns = true;
            }
            else
            {
                q.ColumnSet.AddColumns(columns.ToArray());
            }
            foreach (var item in key_value)
            {
                q.Criteria.AddCondition(item.Key, ConditionOperator.Equal, item.Value);
            }

            return finder.Retrieve(q, ignorePermissions);
        }
    }
}