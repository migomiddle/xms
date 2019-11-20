using System.Collections.Generic;
using System.Linq;
using Xms.Infrastructure.Utility;
using Xms.Schema.Extensions;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Extensions
{
    /// <summary>
    /// 查询表达式扩展类
    /// </summary>
    public static class QueryExpressionExtensions
    {
        public static LinkEntity FindLinkEntityByAlias(this QueryExpression query, string alias)
        {
            var result = query.LinkEntities.Find(n => n.EntityAlias.IsCaseInsensitiveEqual(alias));
            if (result == null)
            {
                foreach (var le in query.LinkEntities)
                {
                    if (le.LinkEntities.NotEmpty())
                    {
                        result = le.LinkEntities.Find(n => n.EntityAlias.IsCaseInsensitiveEqual(alias));
                    }
                }
            }

            return result;
        }

        public static LinkEntity FindLinkEntityByName(this QueryExpression query, string name)
        {
            var result = query.LinkEntities.Find(n => n.LinkToEntityName.IsCaseInsensitiveEqual(name));
            if (result == null)
            {
                foreach (var le in query.LinkEntities)
                {
                    if (le.LinkEntities.NotEmpty())
                    {
                        result = le.LinkEntities.Find(n => n.EntityAlias.IsCaseInsensitiveEqual(name));
                    }
                }
            }

            return result;
        }

        public static List<string> GetAllEntityNames(this QueryExpression query, LinkEntity parent = null)
        {
            var result = new List<string>();
            if (parent == null)
            {
                result.Add(query.EntityName);
                if (query.LinkEntities.NotEmpty())
                {
                    foreach (var le in query.LinkEntities)
                    {
                        if (le.LinkEntities.NotEmpty())
                        {
                            GetAllEntityNames(query, le);
                        }
                        else
                        {
                            result.Add(le.LinkToEntityName);
                        }
                    }
                }
            }
            else
            {
                result.Add(parent.LinkToEntityName);
                if (parent.LinkEntities.NotEmpty())
                {
                    foreach (var le in parent.LinkEntities)
                    {
                        if (le.LinkEntities.NotEmpty())
                        {
                            GetAllEntityNames(query, le);
                        }
                        else
                        {
                            result.Add(le.LinkToEntityName);
                        }
                    }
                }
            }

            return result;
        }

        public static List<string> GetAllColumns(this QueryExpression query, LinkEntity lEntity = null, List<Schema.Domain.Attribute> attributeMetaDatas = null, bool wrapName = true, string aliasJoiner = ".")
        {
            List<string> result = new List<string>();
            if (lEntity != null)
            {
                if (lEntity.Columns.AllColumns)
                {
                    lEntity.Columns.Columns.Clear();
                    lEntity.Columns.AddColumns(attributeMetaDatas.Where(n => n.EntityName.IsCaseInsensitiveEqual(lEntity.LinkToEntityName)).Select(n => n.Name.ToLower()).ToArray());
                }
                foreach (var column in lEntity.Columns.Columns)
                {
                    if (wrapName)
                    {
                        var attr = attributeMetaDatas.Find(n => n.EntityName.IsCaseInsensitiveEqual(lEntity.LinkToEntityName) && n.Name.IsCaseInsensitiveEqual(column));
                        if (attr != null && (attr.TypeIsLookUp() || attr.TypeIsOwner() || attr.TypeIsCustomer() || attr.TypeIsState() || attr.TypeIsBit() || attr.TypeIsPickList()))
                        {
                            result.Add((lEntity.EntityAlias + aliasJoiner + column + "name").ToLower());
                        }
                    }
                    result.Add((lEntity.EntityAlias + aliasJoiner + column).ToLower());
                }
                if (lEntity.LinkEntities.NotEmpty())
                {
                    foreach (var le in lEntity.LinkEntities)
                    {
                        result.AddRange(query.GetAllColumns(le, attributeMetaDatas, wrapName, aliasJoiner));
                    }
                }
            }
            else
            {
                if (query.ColumnSet.AllColumns)
                {
                    query.ColumnSet.Columns.Clear();
                    query.ColumnSet.AddColumns(attributeMetaDatas.Where(n => n.EntityName.IsCaseInsensitiveEqual(query.EntityName)).Select(n => n.Name).ToArray());
                }
                foreach (var column in query.ColumnSet.Columns)
                {
                    if (wrapName)
                    {
                        var attr = attributeMetaDatas.Find(n => n.EntityName.IsCaseInsensitiveEqual(query.EntityName) && n.Name.IsCaseInsensitiveEqual(column));
                        if (attr != null && (attr.TypeIsLookUp() || attr.TypeIsOwner() || attr.TypeIsCustomer() || attr.TypeIsState() || attr.TypeIsBit() || attr.TypeIsPickList()))
                        {
                            result.Add(column.ToLower() + "name");
                        }
                    }
                    result.Add(column.ToLower());
                }
                if (query.LinkEntities.NotEmpty())
                {
                    foreach (var le in query.LinkEntities)
                    {
                        result.AddRange(query.GetAllColumns(le, attributeMetaDatas, wrapName, aliasJoiner));
                    }
                }
            }
            if (result.NotEmpty())
            {
                return result.Distinct().ToList();
            }
            return result;
        }
    }
}