using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xms.Context;
using Xms.Core;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data;
using Xms.Data.Provider;
using Xms.Identity;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;
using Xms.Schema.Attribute;
using Xms.Schema.Domain;
using Xms.Schema.Extensions;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Extensions;
using Xms.Sdk.Query;

namespace Xms.Sdk.Data
{
    /// <summary>
    /// 查询表达式解析器
    /// </summary>
    public class QueryExpressionResolver : IQueryResolver
    {
        private readonly IAttributeFinder _attributeFinder;
        private readonly IQueryMetadataFinder _queryMetadataFinder;
        private readonly DataRepositoryBase<dynamic> _repository;
        private readonly IDbContext _dbContext;

        #region fields

        private List<AttributeAlias> _attributeAlias;
        private List<Schema.Domain.Attribute> _attributeList;
        private List<Schema.Domain.Entity> _entityList;
        private List<RelationShip> _relationShipList;
        private QueryExpression _queryExpression = new QueryExpression();

        public string AliasJoiner { get; set; } = ".";

        public List<AttributeAlias> AttributeAliasList
        {
            get
            {
                if (_attributeAlias == null)
                {
                    _attributeAlias = new List<AttributeAlias>();
                }
                return _attributeAlias;
            }
            set
            {
                _attributeAlias = value;
            }
        }

        public List<Schema.Domain.Attribute> AttributeList
        {
            get
            {
                if (_attributeList == null)
                {
                    _attributeList = new List<Schema.Domain.Attribute>();
                }
                return _attributeList;
            }
            set
            {
                _attributeList = value;
            }
        }

        public List<Schema.Domain.Entity> EntityList
        {
            get
            {
                if (_entityList == null)
                {
                    _entityList = new List<Schema.Domain.Entity>();
                }
                return _entityList;
            }
            set
            {
                _entityList = value;
            }
        }

        public Schema.Domain.Entity MainEntity
        {
            get
            {
                if (_entityList.NotEmpty())
                {
                    return _entityList[0];
                }
                return null;
            }
            set { }
        }

        public QueryParameters Parameters { get; set; } = new QueryParameters();

        public QueryBase QueryObject
        {
            get { return _queryExpression; }
            set { _queryExpression = value as QueryExpression; }
        }

        public List<RelationShip> RelationShipList
        {
            get
            {
                if (_relationShipList == null)
                {
                    _relationShipList = new List<RelationShip>();
                }
                return _relationShipList;
            }
            set
            {
                _relationShipList = value;
            }
        }

        public ICurrentUser User { get; set; }

        #endregion fields

        #region ctor

        public QueryExpressionResolver(IAppContext appContext
            , IDbContext dbContext
            , IQueryMetadataFinder queryMetadataFinder
            , IAttributeFinder attributeFinder)
        {
            User = appContext.GetFeature<ICurrentUser>();
            _dbContext = dbContext;
            _repository = new DataRepositoryBase<dynamic>(_dbContext);
            _queryMetadataFinder = queryMetadataFinder;
            _attributeFinder = attributeFinder;
        }

        public IQueryResolver Init(QueryBase query)
        {
            _queryExpression = query as QueryExpression;
            if (query != null && query.EntityName.IsNotEmpty())
            {
                var metadatas = _queryMetadataFinder.GetAll(query);
                EntityList = metadatas.entities;
                AttributeList = metadatas.attributes;
                RelationShipList = metadatas.relationShips;
            }
            return this;
        }

        #endregion ctor

        #region query

        public dynamic Find(bool ignorePermissions = false, List<Schema.Domain.Attribute> noneReadFields = null)
        {
            var result = _repository.Find(ToSqlString(ignorePermissions: ignorePermissions, noneReadFields: noneReadFields), Parameters.Args.ToArray());
            return result;
        }

        public List<dynamic> Query(bool ignorePermissions = false, List<Schema.Domain.Attribute> noneReadFields = null)
        {
            var result = _repository.ExecuteQuery(ToSqlString(ignorePermissions: ignorePermissions, noneReadFields: noneReadFields), Parameters.Args.ToArray());
            return result;
        }

        public PagedList<dynamic> QueryPaged(bool ignorePermissions = false, List<Schema.Domain.Attribute> noneReadFields = null)
        {
            var result = _repository.ExecuteQueryPaged(_queryExpression.PageInfo.PageNumber, _queryExpression.PageInfo.PageSize, ToSqlString(ignorePermissions: ignorePermissions, noneReadFields: noneReadFields), Parameters.Args.ToArray());
            return result;
        }

        #endregion query

        #region generate sql

        public string ToSqlString(bool includeNameField = true, bool ignorePermissions = false, List<Schema.Domain.Attribute> noneReadFields = null)
        {
            Parameters.Args.Clear();
            if (_queryExpression.ColumnSet.AllColumns && _queryExpression.ColumnSet.Columns.IsEmpty())
            {
                _queryExpression.ColumnSet.AddColumns(AttributeList.Where(n => n.EntityName.IsCaseInsensitiveEqual(_queryExpression.EntityName)).Select(f => f.Name.ToLower()).ToArray());
            }
            var primaryAttr = AttributeList.Find(n => n.EntityName.IsCaseInsensitiveEqual(_queryExpression.EntityName) && n.TypeIsPrimaryKey());
            if (primaryAttr == null)
            {
                primaryAttr = _attributeFinder.Find(n => n.EntityName == _queryExpression.EntityName && n.AttributeTypeName == AttributeTypeIds.PRIMARYKEY);
            }
            if (!_queryExpression.ColumnSet.AllColumns && _queryExpression.ColumnSet.Columns.Find(x => x.IsCaseInsensitiveEqual(primaryAttr.Name)) == null)
            {
                _queryExpression.ColumnSet.AddColumn(primaryAttr.Name.ToLower());
            }
            StringBuilder sqlString = new StringBuilder();
            List<string> tableList = new List<string>();
            List<string> attrList = new List<string>();
            List<string> filterList = new List<string>();
            List<string> orderList = new List<string>();
            string mainEntityAlias = _queryExpression.EntityName;
            //tables
            tableList.Add("[" + _queryExpression.EntityName + "View] AS [" + mainEntityAlias + "]" + (_queryExpression.NoLock ? " WITH(NOLOCK)" : ""));
            //columns
            foreach (var column in _queryExpression.ColumnSet.Columns)
            {
                var field = column.ToLower();
                var attr = AttributeList.Find(x => x.Name.IsCaseInsensitiveEqual(column) && x.EntityName.IsCaseInsensitiveEqual(_queryExpression.EntityName));
                if (_queryExpression.ColumnSet.ColumnFormatting.ContainsKey(field))//if has formatting
                {
                    var formatting = _queryExpression.ColumnSet.ColumnFormatting[field];
                    attrList.Add("[" + formatting + "] AS [" + column.ToLower() + "_formatting]");
                }
                if (attr != null)
                {
                    bool nullRead = noneReadFields != null ? noneReadFields.Exists(x => x.AttributeId == attr.AttributeId) : false;
                    AttributeAliasList.Add(new AttributeAlias() { EntityName = _queryExpression.EntityName, EntityAlias = mainEntityAlias, Name = attr.Name, Alias = attr.Name });

                    //guid类型的字段值，将包含对应的名称字段
                    if (includeNameField && (attr.TypeIsLookUp() || attr.TypeIsOwner() || attr.TypeIsCustomer()))
                    {
                        field += "name";
                        field = PocoHelper.WrapName(field);
                        if (nullRead)
                        {
                            attrList.Add("NULL AS " + field);
                        }
                        else
                        {
                            attrList.Add(PocoHelper.WrapName(mainEntityAlias) + "." + field);
                        }
                    }
                    if (attr.TypeIsState() || attr.TypeIsBit())
                    {
                        if (nullRead)
                        {
                            attrList.Add("NULL AS " + PocoHelper.WrapName(field));
                        }
                        else
                        {
                            attrList.Add(PocoHelper.WrapName(mainEntityAlias) + "." + PocoHelper.WrapName(field));
                            field = string.Format("(CASE {0} WHEN 0 THEN '{1}' WHEN 1 THEN '{2}' ELSE '' END) AS {3}"
                                , PocoHelper.WrapName(mainEntityAlias) + "." + PocoHelper.WrapName(field)
                                , attr.PickLists.Find(n => n.Value == 0).Name, attr.PickLists.Find(n => n.Value == 1).Name
                                , PocoHelper.WrapName(field + "Name"));
                            attrList.Add(field);
                        }
                    }
                    else if (attr.TypeIsPickList() || attr.TypeIsStatus())
                    {
                        if (nullRead)
                        {
                            attrList.Add("NULL AS " + field);
                            attrList.Add(string.Format("NULL AS {0}", PocoHelper.WrapName(field + "Name")));
                        }
                        else
                        {
                            attrList.Add(PocoHelper.WrapName(mainEntityAlias) + "." + PocoHelper.WrapName(field));
                            if (attr.OptionSet.Items.NotEmpty())
                            {
                                var pField = string.Format("(CASE {0}", PocoHelper.WrapName(mainEntityAlias) + "." + PocoHelper.WrapName(field));
                                foreach (var ops in attr.OptionSet.Items)
                                {
                                    pField += string.Format(" WHEN {0} THEN '{1}' ", ops.Value, ops.Name);
                                }
                                pField += string.Format("ELSE '' END) AS {0}", PocoHelper.WrapName(field + "Name"));
                                attrList.Add(pField);
                            }
                            else
                            {
                                attrList.Add(string.Format("'' AS {0}name", PocoHelper.WrapName(field + "Name")));
                            }
                        }
                    }
                    else if (attr.TypeIsDecimal() || attr.TypeIsFloat() || attr.TypeIsMoney())
                    {
                        if (nullRead)
                        {
                            attrList.Add("NULL AS " + PocoHelper.WrapName(field));
                        }
                        else
                        {
                            attrList.Add("CONVERT(DECIMAL(18," + attr.Precision + "),ISNULL(" + PocoHelper.WrapName(mainEntityAlias) + "." + PocoHelper.WrapName(field) + ",0))  AS " + PocoHelper.WrapName(field));
                        }
                    }
                    else if (attr.TypeIsDateTime())
                    {
                        if (nullRead)
                        {
                            attrList.Add("NULL AS " + PocoHelper.WrapName(field));
                        }
                        else
                        {
                            if (attr.DataFormat.IsCaseInsensitiveEqual("yyyy/MM/dd"))
                            {
                                attrList.Add("CONVERT(CHAR(10), " + PocoHelper.WrapName(mainEntityAlias) + "." + PocoHelper.WrapName(field) + ", 121)  AS " + PocoHelper.WrapName(field));
                            }
                            else
                            {
                                attrList.Add(PocoHelper.WrapName(mainEntityAlias) + "." + PocoHelper.WrapName(field));
                            }
                        }
                    }
                    else
                    {
                        if (nullRead)
                        {
                            attrList.Add("NULL AS " + PocoHelper.WrapName(column));
                        }
                        else
                        {
                            attrList.Add(PocoHelper.WrapName(mainEntityAlias) + "." + PocoHelper.WrapName(column));
                        }
                    }
                }
                //else
                //{
                //    attrList.Add(mainEntityAlias + "." + column);
                //}
            }
            //primary field
            var primaryField = AttributeList.Find(n => n.EntityName.IsCaseInsensitiveEqual(_queryExpression.EntityName) && n.IsPrimaryField);
            if (primaryField != null && includeNameField
                && !AttributeAliasList.Exists(x => x.EntityName.IsCaseInsensitiveEqual(_queryExpression.EntityName) && x.EntityAlias.IsCaseInsensitiveEqual(mainEntityAlias)
                && x.Name.IsCaseInsensitiveEqual(primaryField.Name) && x.Alias.IsCaseInsensitiveEqual(primaryField.Name)))
            {
                attrList.Add(PocoHelper.WrapName(mainEntityAlias) + "." + PocoHelper.WrapName(primaryField.Name));
                AttributeAliasList.Add(new AttributeAlias() { EntityName = _queryExpression.EntityName, EntityAlias = mainEntityAlias, Name = primaryField.Name, Alias = primaryField.Name });
            }
            //filters
            if (!ignorePermissions && MainEntity.AuthorizationEnabled && User != null && !User.IsSuperAdmin)
            {
                if (User.RoleObjectAccessEntityPermission.IsEmpty())
                {
                    throw new XmsUnauthorizedException("没有 '" + MainEntity.LocalizedName + "' 读取权限");
                }
                var a = User.RoleObjectAccessEntityPermission.Where(n => n.EntityId == MainEntity.EntityId).OrderByDescending(n => n.AccessRightsMask).ToList();
                var prv = a.NotEmpty() ? a.First() : null;
                if (prv == null || prv.AccessRightsMask == EntityPermissionDepth.None)
                {
                    throw new XmsUnauthorizedException("没有 '" + MainEntity.Name + "' 读取权限");
                }
                filterList.Add("((");
                //basic
                if (prv != null && MainEntity.EntityMask == EntityMaskEnum.User && prv.AccessRightsMask == Core.EntityPermissionDepth.Self)
                {
                    filterList.Add(string.Format("{0}.[OwnerIdType] = 1 AND {0}.[OwnerId]=@0", PocoHelper.WrapName(mainEntityAlias)));
                    Parameters.Args.Add(User.SystemUserId);
                }
                //local
                else if (prv != null && MainEntity.EntityMask == EntityMaskEnum.User && prv.AccessRightsMask == Core.EntityPermissionDepth.BusinessUnit)
                {
                    filterList.Add(string.Format("{0}.[OwnerIdType] = 1 AND {0}.[OwningBusinessUnit]=@0", PocoHelper.WrapName(mainEntityAlias)));
                    Parameters.Args.Add(User.BusinessUnitId);
                }
                //deep
                else if (prv != null && MainEntity.EntityMask == EntityMaskEnum.User && prv.AccessRightsMask == Core.EntityPermissionDepth.BusinessUnitAndChild)
                {
                    filterList.Add(string.Format("{0}.[OwnerIdType] = 1 AND {0}.[OwningBusinessUnit] IN(SELECT [BusinessUnitId] FROM dbo.ufn_Org_GetDeptTree(@0))", PocoHelper.WrapName(mainEntityAlias)));
                    Parameters.Args.Add(User.BusinessUnitId);
                }
                //full
                else if (prv != null && prv.AccessRightsMask == EntityPermissionDepth.Organization)
                {
                    filterList.Add(string.Format("{0}.[OrganizationId]=@0", PocoHelper.WrapName(mainEntityAlias)));
                    Parameters.Args.Add(User.OrganizationId);
                }
                filterList.Add(")");
                //team owner
                if (MainEntity.EntityMask == EntityMaskEnum.User)
                {
                    filterList.Add(string.Format("OR ({0}.[OwnerIdType] = 3 AND {0}.[OwnerId] IN(SELECT [TeamId] FROM [TeamMembership] WHERE [SystemUserId] = @1))", PocoHelper.WrapName(mainEntityAlias)));
                }
                Parameters.Args.Add(User.SystemUserId);
                //shared
                filterList.Add(string.Format(" OR {0}.{1} IN(SELECT [ObjectId] FROM [PrincipalObjectAccess] WHERE [PrincipalId]=@2 AND [EntityId]=@3 AND {0}.{1}=[ObjectId] AND [AccessRightsMask] = @4)", PocoHelper.WrapName(mainEntityAlias), PocoHelper.WrapName(primaryAttr.Name)));
                filterList.Add(")");
                Parameters.Args.Add(User.SystemUserId);
                Parameters.Args.Add(MainEntity.EntityId);
                Parameters.Args.Add((int)AccessRightValue.Read);
            }
            ParseFilter(EntityList.Find(n => n.Name.IsCaseInsensitiveEqual(_queryExpression.EntityName)), _queryExpression.Criteria.FilterOperator, _queryExpression.Criteria, mainEntityAlias, ref filterList);
            //link entities
            foreach (var le in _queryExpression.LinkEntities)
            {
                le.FromEntityAlias = mainEntityAlias;
                ParseLinkEntity(le, ref tableList, ref attrList, ref filterList, includeNameField);
            }
            //orders
            foreach (var ord in _queryExpression.Orders)
            {
                var ordName = ord.AttributeName;
                if (ord.AttributeName.IndexOf(".") > 0) //has alias
                {
                    var b = ord.AttributeName.Split('.');
                    var rs = RelationShipList.Find(n => n.Name.IsCaseInsensitiveEqual(b[0]));
                    var a = AttributeList.Find(n => n.Name.IsCaseInsensitiveEqual(b[1]) && n.EntityName.IsCaseInsensitiveEqual(b[0]));
                    if (null == a)
                    {
                        if (ord.AttributeName.EndsWith("name", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var nm = b[1].Remove(b[1].Length - 4, 4);
                            a = AttributeList.Find(x => x.EntityId == rs.ReferencedEntityId && x.Name == nm);
                            if (null != a)
                            {
                                //ordName = a.Name;
                                b[1] = a.Name;
                            }
                        }
                    }
                    if (null != a)
                    {
                        orderList.Add(b[0] + "." + b[1] + (ord.OrderType == OrderType.Descending ? " DESC" : ""));
                    }
                }
                else
                {
                    var a = AttributeList.Find(n => n.Name.IsCaseInsensitiveEqual(ord.AttributeName) && n.EntityName.IsCaseInsensitiveEqual(MainEntity.Name));
                    if (null == a)
                    {
                        if (ord.AttributeName.EndsWith("name", StringComparison.InvariantCultureIgnoreCase))
                        {
                            a = AttributeList.Find(n => n.Name.IsCaseInsensitiveEqual(ord.AttributeName.Remove(ord.AttributeName.Length - 4, 4)));
                            if (null != a)
                            {
                                ordName = a.Name;
                            }
                        }
                    }
                    if (null != a)
                    {
                        orderList.Add(mainEntityAlias + "." + ordName + (ord.OrderType == OrderType.Descending ? " DESC" : ""));
                    }
                }
            }
            string tableStr = string.Join(" ", tableList);
            string attrStr = string.Join(",", attrList).ToLower();
            string filterStr = string.Join(" ", filterList);
            string orderStr = string.Join(",", orderList);
            sqlString.AppendFormat("SELECT {0} {1} FROM {2} {3} {4} ", _queryExpression.Distinct ? "DISTINCT" : "", attrStr
                , tableStr, filterStr.IsNotEmpty() ? " WHERE " + filterStr : ""
                , orderStr.IsNotEmpty() ? " ORDER BY " + orderStr : "");
            if (_queryExpression.Distinct)
            {
                return string.Format("SELECT * FROM ({0}) a", sqlString.ToString());
            }
            return sqlString.ToString();
        }

        private string GetLinkType(JoinOperator joinOperator)
        {
            string join = "LEFT JOIN";
            switch (joinOperator)
            {
                case JoinOperator.Inner:
                    join = "INNER JOIN";
                    break;

                case JoinOperator.LeftOuter:
                    join = "LEFT JOIN";
                    break;

                case JoinOperator.Natural:
                    join = "INNER JOIN";
                    break;
            }
            return join;
        }

        /// <summary>
        ///生成过滤条件
        /// </summary>
        /// <param name="logicalOperator"></param>
        /// <param name="filter"></param>
        /// <param name="entityAlias"></param>
        /// <param name="filterList"></param>
        private void ParseFilter(Schema.Domain.Entity entityMetaData, LogicalOperator logicalOperator, FilterExpression filter, string entityAlias, ref List<string> filterList)
        {
            bool flag = false;
            if (filter.Conditions.NotEmpty())
            {
                filterList.Add((filterList.NotEmpty() ? (logicalOperator == LogicalOperator.And ? "AND" : "OR") : string.Empty) + " (");
                foreach (var cd in filter.Conditions)
                {
                    if (flag)
                    {
                        filterList.Add(filter.FilterOperator == LogicalOperator.And ? "AND" : "OR");
                    }
                    filterList.Add(MakeCondition(entityMetaData, entityAlias, cd));
                    flag = true;
                }
                if (filter.Filters.NotEmpty())
                {
                    foreach (var item in filter.Filters)
                    {
                        ParseFilter(entityMetaData, filter.FilterOperator, item, entityAlias, ref filterList);
                    }
                }
                filterList.Add(")");
            }
            else if (filter.Filters.NotEmpty())
            {
                foreach (var item in filter.Filters)
                {
                    ParseFilter(entityMetaData, filter.FilterOperator, item, entityAlias, ref filterList);
                }
            }
        }

        private string MakeCondition(Schema.Domain.Entity entityMetaData, string entityAliaName, ConditionExpression conditionNode)
        {
            string condition = string.Empty;
            string attrName = (conditionNode.AttributeName.IndexOf('.') < 0 ? PocoHelper.WrapName(entityAliaName) + "." : "") + PocoHelper.WrapName(conditionNode.AttributeName);
            //var attrMeta = AttributeList.Exists(n => n.Name.IsCaseInsensitiveEqual(attrName) && n.EntityName.IsCaseInsensitiveEqual(entityMetaData.Name));
            string value = (conditionNode.Values != null) ? string.Join(",", conditionNode.Values).TrimSafe() : string.Empty;
            string parameter = "@" + Parameters.Args.Count;

            switch (conditionNode.Operator)
            {
                case ConditionOperator.Equal:
                    condition = string.Format("{0}={1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.NotEqual:
                    condition = string.Format("{0}<>{1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.EqualUserId:
                    condition = string.Format("{0}={1}", attrName, parameter);
                    Parameters.Args.Add(User.SystemUserId);
                    break;

                case ConditionOperator.NotEqualUserId:
                    condition = string.Format("{0}<>{1}", attrName, parameter);
                    Parameters.Args.Add(User.SystemUserId);
                    break;

                case ConditionOperator.BeginsWith:
                    condition = string.Format("{0} LIKE {1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.DoesNotBeginWith:
                    condition = string.Format("{0} NOT LIKE {1}", attrName, parameter);
                    value = value + "%";
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.DoesNotContain:
                    condition = string.Format("{0} NOT LIKE {1}", attrName, parameter);
                    value = "%" + value + "%";
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.DoesNotEndWith:
                    condition = string.Format("{0} NOT LIKE {1}", attrName, parameter);
                    value = "%" + value;
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.EndsWith:
                    condition = string.Format("{0} LIKE {1}", attrName, parameter);
                    value = "%" + value;
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.GreaterEqual:
                    condition = string.Format("{0}>={1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.GreaterThan:
                    condition = string.Format("{0}>{1}", attrName, parameter);
                    break;

                case ConditionOperator.LessEqual:
                    condition = string.Format("{0}<={1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.LessThan:
                    condition = string.Format("{0}<{1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                #region date conditions

                case ConditionOperator.Last7Days:
                    condition = string.Format("(DATEDIFF(DAY, {0}, GETDATE())>0 AND DATEDIFF(DAY, {0}, GETDATE())<=7)", attrName);
                    break;

                case ConditionOperator.LastMonth:
                    condition = string.Format("DATEDIFF(MONTH, {0}, GETDATE())=1", attrName);
                    break;

                case ConditionOperator.LastWeek:
                    condition = string.Format("DATEDIFF(WEEK, {0}, GETDATE())=1", attrName);
                    break;

                case ConditionOperator.LastXDays:
                    condition = string.Format("(DATEDIFF(DAY, {0}, GETDATE())>0 AND DATEDIFF(DAY, {0}, GETDATE())<={1})", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.LastXHours:
                    condition = string.Format("(DATEDIFF(HH, {0}, GETDATE())>0 AND DATEDIFF(HH, {0}, GETDATE())<={1})", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.LastXMonths:
                    condition = string.Format("(DATEDIFF(MONTH, {0}, GETDATE())>0 AND DATEDIFF(MONTH, {0}, GETDATE())<={1})", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.LastXWeeks:
                    condition = string.Format("(DATEDIFF(WEEK, {0}, GETDATE())>0 AND DATEDIFF(WEEK, {0}, GETDATE())<={1})", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.LastXYears:
                    condition = string.Format("(DATEDIFF(YEAR, {0}, GETDATE())>0 AND DATEDIFF(YEAR, {0}, GETDATE())<={1})", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.LastYear:
                    condition = string.Format("DATEDIFF(YEAR, {0}, GETDATE())=1", attrName);
                    break;

                case ConditionOperator.Next7Days:
                    condition = string.Format("(DATEDIFF(DAY, GETDATE(), {0})>0 AND DATEDIFF(DAY, GETDATE(), {0})<=7)", attrName);
                    break;

                case ConditionOperator.NextMonth:
                    condition = string.Format("DATEDIFF(MONTH, GETDATE(), {0})=1", attrName);
                    break;

                case ConditionOperator.NextWeek:
                    condition = string.Format("DATEDIFF(WEEK, GETDATE(), {0})=1", attrName);
                    break;

                case ConditionOperator.NextXDays:
                    condition = string.Format("(DATEDIFF(DAY, GETDATE(), {0})>0 AND DATEDIFF(DAY, GETDATE(), {0})<={1})", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.NextXHours:
                    condition = string.Format("(DATEDIFF(HOUR, GETDATE(), {0})>0 AND DATEDIFF(HOUR, GETDATE(), {0})<={1})", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.NextXMonths:
                    condition = string.Format("(DATEDIFF(MONTH, GETDATE(), {0})>0 AND DATEDIFF(MONTH, GETDATE(), {0})<={1})", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.NextXWeeks:
                    condition = string.Format("(DATEDIFF(WEEK, GETDATE(), {0})>0 AND DATEDIFF(WEEK, GETDATE(), {0})<={1})", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.NextXYears:
                    condition = string.Format("(DATEDIFF(YEAR, GETDATE(), {0})>0 AND DATEDIFF(YEAR, GETDATE(), {0})<={1})", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.NextYear:
                    condition = string.Format("DATEDIFF(YEAR, GETDATE(), {0})=1", attrName);
                    break;

                case ConditionOperator.Today:
                    condition = string.Format("DATEDIFF(DAY,{0},GETDATE())=0", attrName);
                    break;

                case ConditionOperator.NotBetween:
                    condition = string.Format("{0} NOT BETWEEN '{1}' AND '{2}'", attrName, conditionNode.Values[0], conditionNode.Values[1]);
                    break;

                case ConditionOperator.Between:
                    condition = string.Format("{0} BETWEEN '{1}' AND '{2}'", attrName, conditionNode.Values[0], conditionNode.Values[1]);
                    break;

                case ConditionOperator.OlderThanXMonths:
                    condition = string.Format("DATEDIFF(MONTH, {0}, GETDATE())>={1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.On:
                    condition = string.Format("{0}={1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.NotOn:
                    condition = string.Format("{0}<>{1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.OnOrAfter:
                    condition = string.Format("DATEDIFF(DAY, {0}, GETDATE())<={1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.OnOrBefore:
                    condition = string.Format("DATEDIFF(DAY, {0}, GETDATE())>={1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.After:
                    condition = string.Format("DATEDIFF(DAY, {0}, GETDATE())<{1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.Before:
                    condition = string.Format("DATEDIFF(DAY, {0}, GETDATE())>{1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.OnOrAfterToday:
                    condition = string.Format("DATEDIFF(DAY, {0}, GETDATE())<=0", attrName, parameter);
                    break;

                case ConditionOperator.OnOrBeforeToday:
                    condition = string.Format("DATEDIFF(DAY, {0}, GETDATE())>=0", attrName, parameter);
                    break;

                case ConditionOperator.AfterToday:
                    condition = string.Format("DATEDIFF(DAY, {0}, GETDATE())<0", attrName, parameter);
                    break;

                case ConditionOperator.BeforeToday:
                    condition = string.Format("DATEDIFF(DAY, {0}, GETDATE())>0", attrName, parameter);
                    break;

                case ConditionOperator.ThisMonth:
                    condition = string.Format("DATEDIFF(MONTH, {0}, GETDATE())=0", attrName);
                    break;

                case ConditionOperator.ThisWeek:
                    condition = string.Format("DATEDIFF(WEEK, {0}, GETDATE())=0", attrName);
                    break;

                case ConditionOperator.ThisYear:
                    condition = string.Format("DATEDIFF(YEAR, {0}, GETDATE())=0", attrName);
                    break;

                case ConditionOperator.Tomorrow:
                    condition = string.Format("DATEDIFF(MONTH, {0}, GETDATE())=-1", attrName);
                    break;

                case ConditionOperator.Yesterday:
                    condition = string.Format("DATEDIFF(MONTH, {0}, GETDATE())=1", attrName);
                    break;

                case ConditionOperator.OlderThanXYears:
                    condition = string.Format("DATEDIFF(YEAR, {0}, GETDATE())>={1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.OlderThanXDays:
                    condition = string.Format("DATEDIFF(DAY, {0}, GETDATE())>={1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.AfterXYears:
                    condition = string.Format("DATEDIFF(YEAR, {0}, GETDATE())<={1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.AfterXMonths:
                    condition = string.Format("DATEDIFF(MONTH, {0}, GETDATE())<={1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.AfterXDays:
                    condition = string.Format("DATEDIFF(DAY, {0}, GETDATE())<={1}", attrName, parameter);
                    Parameters.Args.Add(value);
                    break;

                #endregion date conditions

                case ConditionOperator.NotIn:
                    condition = string.Format("{0} NOT IN({1})", attrName, conditionNode.Values.CollectionToString(",", "'"));
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.In:
                    condition = string.Format("{0} IN({1})", attrName, conditionNode.Values.CollectionToString(",", "'"));
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.Like:
                    condition = string.Format("{0} LIKE {1}", attrName, parameter);
                    value = "%" + value + "%";
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.Contains:
                    condition = string.Format("{0} LIKE {1}", attrName, parameter);
                    value = "%" + value + "%";
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.NotLike:
                    condition = string.Format("{0} NOT LIKE {1}", attrName, parameter);
                    value = "%" + value + "%";
                    Parameters.Args.Add(value);
                    break;

                case ConditionOperator.Null:
                    condition = string.Format("{0} IS NULL", attrName);
                    break;

                case ConditionOperator.NotNull:
                    condition = string.Format("{0} IS NOT NULL", attrName);
                    break;

                case ConditionOperator.EqualBusinessId:
                    condition = string.Format("{0}={1}", attrName, parameter);
                    Parameters.Args.Add(User.BusinessUnitId);
                    break;

                case ConditionOperator.NotEqualBusinessId:
                    condition = string.Format("{0}<>{1}", attrName, parameter);
                    Parameters.Args.Add(User.BusinessUnitId);
                    break;

                case ConditionOperator.EqualOrganizationId:
                    condition = string.Format("{0}={1}", attrName, parameter);
                    Parameters.Args.Add(User.OrganizationId);
                    break;

                case ConditionOperator.NotEqualOrganizationId:
                    condition = string.Format("{0}<>{1}", attrName, parameter);
                    Parameters.Args.Add(User.OrganizationId);
                    break;

                default:
                    break;
            }

            return condition;
        }

        private void ParseLinkEntity(LinkEntity linkEntity, ref List<string> tableList, ref List<string> attrList, ref List<string> filterList, bool includeNameField = true, List<Schema.Domain.Attribute> noneReadFields = null)
        {
            if (linkEntity.Columns.AllColumns && linkEntity.Columns.Columns.IsEmpty())
            {
                linkEntity.Columns.AddColumns(AttributeList.Where(n => n.EntityName.IsCaseInsensitiveEqual(linkEntity.LinkToEntityName)).Select(f => f.Name).ToArray());
            }
            string entityAlias = linkEntity.EntityAlias;
            foreach (var column in linkEntity.Columns.Columns)
            {
                //guid类型的字段值，将替换为对应的名称字段//link entity like alias.name
                var field = column.ToLower();
                var attr = AttributeList.Find(x => x.Name.IsCaseInsensitiveEqual(column) && x.EntityName.IsCaseInsensitiveEqual(linkEntity.LinkToEntityName));
                if (linkEntity.Columns.ColumnFormatting.ContainsKey(field))//if has formatting
                {
                    var formatting = linkEntity.Columns.ColumnFormatting[field];
                    attrList.Add((PocoHelper.WrapName(formatting) + " AS '" + entityAlias + AliasJoiner + field + "_Formatting'").ToLower());
                }
                if (attr != null)
                {
                    bool nullRead = noneReadFields != null ? noneReadFields.Exists(x => x.AttributeId == attr.AttributeId) : false;
                    AttributeAliasList.Add(new AttributeAlias() { EntityName = linkEntity.LinkToEntityName, EntityAlias = entityAlias, Name = attr.Name, Alias = entityAlias + AliasJoiner + attr.Name });
                    SetFieldSegment(attrList, attr, entityAlias, field, nullRead, includeNameField, GetColumnAsSegment);
                    //if (includeNameField && (attr.TypeIsPrimaryKey() || attr.TypeIsLookUp() || attr.TypeIsOwner() || attr.TypeIsCustomer()))
                    //{
                    //    if (attr.TypeIsPrimaryKey())
                    //    {
                    //        field = "name";
                    //    }
                    //    else
                    //    {
                    //        field += "name";
                    //    }
                    //    if (nullRead)
                    //    {
                    //        attrList.Add("NULL AS '" + (entityAlias + AliasJoiner + field).ToLower() + "'");
                    //    }
                    //    else
                    //    {
                    //        attrList.Add(entityAlias + "." + field + " AS '" + (entityAlias + AliasJoiner + field).ToLower() + "'");
                    //    }
                    //}
                    //if (attr.TypeIsState() || attr.TypeIsBit())
                    //{
                    //    if (nullRead)
                    //    {
                    //        attrList.Add("NULL AS '" + entityAlias + "." + field + "'");
                    //        attrList.Add(string.Format("NULL AS '{0}Name'", entityAlias + AliasJoiner + field));
                    //    }
                    //    else
                    //    {
                    //        attrList.Add(entityAlias + "." + field + " AS '" + entityAlias + "." + field + "'");
                    //        field = string.Format("(CASE {0} WHEN 0 THEN '{1}' WHEN 1 THEN '{2}' ELSE '' END) AS '{3}Name'", entityAlias + "." + field, attr.PickLists.Find(n => n.Value == 0).Name
                    //            , attr.PickLists.Find(n => n.Value == 1).Name
                    //            , entityAlias + AliasJoiner + field);
                    //        attrList.Add(field);
                    //    }
                    //}
                    //else if (attr.TypeIsPickList())
                    //{
                    //    if (nullRead)
                    //    {
                    //        attrList.Add("NULL AS '" + entityAlias + "." + field + "'");
                    //        attrList.Add(string.Format("NULL AS '{0}Name'", entityAlias + AliasJoiner + field));
                    //    }
                    //    else
                    //    {
                    //        attrList.Add(entityAlias + "." + field + " AS '" + entityAlias + "." + field + "'");
                    //        var pField = string.Format("(CASE {0}", entityAlias + "." + field);
                    //        foreach (var ops in attr.OptionSet.Items)
                    //        {
                    //            pField += string.Format(" WHEN {0} THEN '{1}' ", ops.Value, ops.Name);
                    //        }
                    //        pField += string.Format("ELSE '' END) AS '{0}name'", entityAlias + AliasJoiner + field);
                    //        attrList.Add(pField);
                    //    }
                    //}
                    //else if (attr.TypeIsDecimal() || attr.TypeIsFloat() || attr.TypeIsMoney())
                    //{
                    //    if (nullRead)
                    //    {
                    //        attrList.Add(string.Format("NULL AS '{0}'", entityAlias + AliasJoiner + field));
                    //    }
                    //    else
                    //    {
                    //        attrList.Add("CONVERT(DECIMAL(18," + attr.Precision + "),ISNULL(" + entityAlias + "." + field + ",0)) AS '" + entityAlias + AliasJoiner + field + "'");
                    //    }
                    //}
                    //else
                    //{
                    //    if (nullRead)
                    //    {
                    //        attrList.Add(string.Format("NULL AS '{0}'", entityAlias + AliasJoiner + column));
                    //    }
                    //    else
                    //    {
                    //        attrList.Add(entityAlias + "." + column + " AS '" + entityAlias + AliasJoiner + column + "'");
                    //    }
                    //}
                }
            }
            var entity = EntityList.Find(n => n.Name.IsCaseInsensitiveEqual(linkEntity.LinkToEntityName));
            var joinCondition = PocoHelper.WrapName(entityAlias) + "." + PocoHelper.WrapName(linkEntity.LinkToAttributeName) + " = " + PocoHelper.WrapName(linkEntity.FromEntityAlias.IfEmpty(linkEntity.LinkFromEntityName)) + "." + PocoHelper.WrapName(linkEntity.LinkFromAttributeName);
            if (entity.AuthorizationEnabled && User != null)
            {
                if (!User.IsSuperAdmin && entity.AuthorizationEnabled && User.RoleObjectAccessEntityPermission.IsEmpty())
                {
                    joinCondition += " AND 1=2";
                }
                else if (!User.IsSuperAdmin)
                {
                    var prv = User.RoleObjectAccessEntityPermission.Find(n => n.EntityId == entity.EntityId && n.AccessRight == Core.AccessRightValue.Read);
                    //basic
                    if (prv != null && entity.EntityMask == EntityMaskEnum.User && prv.AccessRightsMask == Core.EntityPermissionDepth.Self)
                    {
                        joinCondition += string.Format(" AND {0}.[OwnerId]=@{1}", PocoHelper.WrapName(entityAlias), Parameters.Args.Count);
                        Parameters.Args.Add(User.SystemUserId);
                    }
                    //local
                    else if (prv != null && entity.EntityMask == EntityMaskEnum.User && prv.AccessRightsMask == Core.EntityPermissionDepth.BusinessUnit)
                    {
                        joinCondition += string.Format(" AND {0}.[OwningBusinessUnit]=@{1}", PocoHelper.WrapName(entityAlias), Parameters.Args.Count);
                        Parameters.Args.Add(User.BusinessUnitId);
                    }
                    //deep
                    else if (prv != null && entity.EntityMask == EntityMaskEnum.User && prv.AccessRightsMask == Core.EntityPermissionDepth.BusinessUnitAndChild)
                    {
                        joinCondition += string.Format(" AND {0}.[OwningBusinessUnit] in(SELECT [BusinessUnitId] FROM dbo.ufn_Org_GetDeptTree(@{1}))", entityAlias, Parameters.Args.Count);
                        Parameters.Args.Add(User.BusinessUnitId);
                    }
                    //full
                    else if (prv != null && entity.EntityMask == EntityMaskEnum.Organization)
                    {
                    }
                }
            }
            string tb = GetLinkType(linkEntity.JoinOperator) + " [" + linkEntity.LinkToEntityName + "View] AS " + PocoHelper.WrapName(entityAlias) + (_queryExpression.NoLock ? " WITH(NOLOCK)" : "")
                + " ON " + joinCondition;
            tableList.Add(tb);

            ParseFilter(EntityList.Find(n => n.Name.IsCaseInsensitiveEqual(linkEntity.LinkToEntityName)), linkEntity.LinkCriteria.FilterOperator, linkEntity.LinkCriteria, entityAlias, ref filterList);

            if (linkEntity.LinkEntities.NotEmpty())
            {
                foreach (var le in linkEntity.LinkEntities)
                {
                    ParseLinkEntity(le, ref tableList, ref attrList, ref filterList);
                }
            }
        }

        private void SetFieldSegment(List<string> attrList, Schema.Domain.Attribute attr, string entityAlias, string field, bool nullRead, bool includeNameField, Func<string, string, string> getColumnAsSegment)
        {
            string segment = string.Empty;
            string column = field;
            if (includeNameField && (attr.TypeIsPrimaryKey() || attr.TypeIsLookUp() || attr.TypeIsOwner() || attr.TypeIsCustomer()))
            {
                if (attr.TypeIsPrimaryKey())
                {
                    field = "name";
                }
                else
                {
                    field += "name";
                }
                if (nullRead)
                {
                    attrList.Add("NULL " + getColumnAsSegment(entityAlias, field));
                }
                else
                {
                    attrList.Add(PocoHelper.WrapName(entityAlias) + "." + PocoHelper.WrapName(field) + " " + getColumnAsSegment(entityAlias, field));
                }
            }
            if (attr.TypeIsState() || attr.TypeIsBit())
            {
                if (nullRead)
                {
                    attrList.Add("NULL " + getColumnAsSegment(entityAlias, field));
                    attrList.Add(string.Format("NULL {0}", getColumnAsSegment(entityAlias, field + "Name")));
                }
                else
                {
                    attrList.Add(PocoHelper.WrapName(entityAlias) + "." + PocoHelper.WrapName(field) + " " + getColumnAsSegment(entityAlias, field));
                    field = string.Format("(CASE {0} WHEN 0 THEN '{1}' WHEN 1 THEN '{2}' ELSE '' END) {3}", PocoHelper.WrapName(entityAlias) + "." + PocoHelper.WrapName(field), attr.PickLists.Find(n => n.Value == 0).Name
                        , attr.PickLists.Find(n => n.Value == 1).Name
                        , GetColumnAsSegment(entityAlias, field + "Name"));
                    attrList.Add(field);
                }
            }
            else if (attr.TypeIsPickList())
            {
                if (nullRead)
                {
                    attrList.Add("NULL " + getColumnAsSegment(entityAlias, field));
                    attrList.Add(string.Format("NULL {0}", getColumnAsSegment(entityAlias, field + "Name")));
                }
                else
                {
                    attrList.Add(PocoHelper.WrapName(entityAlias) + "." + PocoHelper.WrapName(field) + " " + getColumnAsSegment(entityAlias, field));
                    var pField = string.Format("(CASE {0}", PocoHelper.WrapName(entityAlias) + "." + PocoHelper.WrapName(field));
                    foreach (var ops in attr.OptionSet.Items)
                    {
                        pField += string.Format(" WHEN {0} THEN '{1}' ", ops.Value, ops.Name);
                    }
                    pField += string.Format("ELSE '' END) {0}", getColumnAsSegment(entityAlias, field + "Name"));
                    attrList.Add(pField);
                }
            }
            else if (attr.TypeIsDecimal() || attr.TypeIsFloat() || attr.TypeIsMoney())
            {
                if (nullRead)
                {
                    attrList.Add(string.Format("NULL {0}", getColumnAsSegment(entityAlias, field)));
                }
                else
                {
                    attrList.Add("CONVERT(DECIMAL(18," + attr.Precision + "),ISNULL(" + PocoHelper.WrapName(entityAlias) + "." + PocoHelper.WrapName(field) + ",0)) " + getColumnAsSegment(entityAlias, field));
                }
            }
            else
            {
                if (nullRead)
                {
                    attrList.Add(string.Format("NULL {0}", getColumnAsSegment(entityAlias, column)));
                }
                else
                {
                    attrList.Add(PocoHelper.WrapName(entityAlias) + "." + PocoHelper.WrapName(column) + " " + getColumnAsSegment(entityAlias, column));
                }
            }
        }

        private string GetColumnAsSegment(string entityAlias, string column)
        {
            return " AS '" + entityAlias + AliasJoiner + column + "'";
        }

        #endregion generate sql
    }
}