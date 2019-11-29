using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core;
using Xms.Core.Components.Platform;
using Xms.Core.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Schema.Abstractions;
using Xms.Schema.Domain;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Schema.OptionSet;
using Xms.Schema.StringMap;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Extensions;
using Xms.Sdk.Query;
using Xms.Security.Principal;

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 查询数据服务
    /// </summary>
    public class FetchDataService : IFetchDataService
    {
        #region fields

        protected readonly IRoleObjectAccessEntityPermissionService _roleObjectAccessEntityPermissionService;
        private readonly ISystemUserRolesService _systemUserRolesService;

        private readonly IAppContext _appContext;
        private readonly IEntityFinder _entityFinder;
        private readonly IStringMapFinder _stringMapFinder;
        private readonly IOptionSetDetailFinder _optionSetDetailFinder;
        private readonly ISystemUserPermissionService _systemUserPermissionService;
        private readonly IQueryResolverFactory _queryResolverFactory;
        public QueryExpression QueryExpression { get; set; } = new QueryExpression();

        public Entity MainEntity
        {
            get
            {
                if (QueryResolver.EntityList.NotEmpty())
                {
                    return QueryResolver.EntityList[0];
                }
                return null;
            }
        }

        public IQueryResolver QueryResolver
        {
            get; set;
        }

        public ICurrentUser User { get; set; }

        public ILocalizedTextProvider Loc
        {
            get
            {
                return _appContext.GetFeature<ILocalizedTextProvider>();
            }
        }

        public List<Schema.Domain.Attribute> NonePermissionFields { get; private set; }

        #endregion fields

        #region ctor

        public FetchDataService(IAppContext appContext
            , IEntityFinder entityFinder
            , IStringMapFinder stringMapFinder
            , IOptionSetDetailFinder optionSetDetailFinder
            , ISystemUserPermissionService systemUserPermissionService
            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService
            , ISystemUserRolesService systemUserRolesService
            , IQueryResolverFactory queryResolverFactory)
        {
            _appContext = appContext;
            User = _appContext.GetFeature<ICurrentUser>();
            _stringMapFinder = stringMapFinder;
            _optionSetDetailFinder = optionSetDetailFinder;
            _systemUserPermissionService = systemUserPermissionService;
            _entityFinder = entityFinder;
            _queryResolverFactory = queryResolverFactory;
            _roleObjectAccessEntityPermissionService = roleObjectAccessEntityPermissionService;
            _systemUserRolesService = systemUserRolesService;
        }

        #endregion ctor

        #region conditions

        private ConditionOperator GetConditionOperator(Schema.Domain.Attribute attr)
        {
            switch (attr.AttributeTypeName)
            {
                case AttributeTypeIds.PRIMARYKEY:
                    return ConditionOperator.Equal;

                case AttributeTypeIds.LOOKUP:
                    return ConditionOperator.Equal;

                case AttributeTypeIds.STATE:
                    return ConditionOperator.Equal;

                case AttributeTypeIds.BIT:
                    return ConditionOperator.Equal;

                case AttributeTypeIds.PICKLIST:
                    return ConditionOperator.Equal;

                case AttributeTypeIds.DATETIME:
                    return ConditionOperator.Equal;

                case AttributeTypeIds.NVARCHAR:
                    return ConditionOperator.Like;

                case AttributeTypeIds.VARCHAR:
                    return ConditionOperator.Like;

                case AttributeTypeIds.INT:
                    return ConditionOperator.Equal;

                default:
                    return ConditionOperator.Equal;
            }
        }

        private string GetSearchName(Schema.Domain.Attribute attr, string entityAlias = "")
        {
            //|| attr.TypeIsState() || attr.TypeIsBit() || attr.TypeIsPickList()
            var name = attr.Name;
            if (attr != null && (attr.TypeIsPrimaryKey() || attr.TypeIsLookUp() || attr.TypeIsOwner()))
            {
                name += "Name";
            }
            return entityAlias.IsEmpty() ? name : entityAlias + "." + name;
        }

        private ConditionExpression GetCondition(Schema.Domain.Attribute attr, object keyword, string entityAlias = "")
        {
            //数字类型
            if (keyword.ToString().IsNumeric() && (attr.TypeIsInt()
                || attr.TypeIsFloat()
                || attr.TypeIsDecimal()
                || attr.TypeIsMoney()
                || attr.TypeIsSmallMoney()
                || attr.TypeIsSmallInt()
                ))
            {
                return new ConditionExpression(GetSearchName(attr, entityAlias), GetConditionOperator(attr), keyword);
            }
            //日期类型
            else if ((attr.TypeIsDateTime()
                || attr.TypeIsSmallDateTime()
                ))
            {
                if (DateTime.TryParse(keyword.ToString(), out DateTime d))
                {
                    return new ConditionExpression(GetSearchName(attr, entityAlias), GetConditionOperator(attr), keyword);
                }
            }
            //guid类型
            else if (attr.TypeIsPrimaryKey()
                                    || attr.TypeIsLookUp()
                                    || attr.TypeIsOwner()
                                    || attr.TypeIsCustomer())
            {
                if (keyword.ToString().IsGuid())
                {
                    return new ConditionExpression(attr.Name, GetConditionOperator(attr), keyword);
                }
                return new ConditionExpression(GetSearchName(attr, entityAlias), ConditionOperator.Like, keyword);
            }
            //选项类型
            else if ((attr.TypeIsState()
                                     || attr.TypeIsBit()
                                     || attr.TypeIsPickList() || attr.TypeIsStatus()))
            {
                if (keyword.ToString().IsInteger())//如果是数值
                {
                    return new ConditionExpression(entityAlias.IsEmpty() ? attr.Name : entityAlias + "." + attr.Name, GetConditionOperator(attr), keyword);
                }
                else
                {
                    //按名称查找选项值
                    //...
                    if (attr.TypeIsState() || attr.TypeIsBit())
                    {
                        if (attr.PickLists.IsEmpty())
                        {
                            attr.PickLists = _stringMapFinder.Query(n => n.Where(f => f.AttributeId == attr.AttributeId));
                        }
                        var value = attr.PickLists.Find(n => n.Name.IsCaseInsensitiveEqual(keyword.ToString()));
                        if (null != value)
                        {
                            keyword = value.Value;
                        }
                    }
                    else if (attr.TypeIsPickList() || attr.TypeIsStatus())
                    {
                        if (attr.OptionSet == null)
                        {
                            attr.OptionSet = new OptionSet();//_optionSetFinder.FindById(attr.OptionSetId.Value);
                        }
                        var value = attr.OptionSet.Items.Find(n => n.Name.IsCaseInsensitiveEqual(keyword.ToString()));
                        if (null != value)
                        {
                            keyword = value.Value;
                        }
                    }
                }
                //if (keyword.ToString().IsInteger())
                //{
                //    return new ConditionExpression(attr.Name, ConditionOperator.Equal, keyword);
                //}
            }
            //字符串类型
            else if ((attr.TypeIsNvarchar()
                                     || attr.TypeIsVarchar()
                                     || attr.TypeIsChar()))
            {
                return new ConditionExpression(GetSearchName(attr, entityAlias), GetConditionOperator(attr), keyword);
            }
            return null;
        }

        /// <summary>
        /// 根据关键字生成过滤条件
        /// </summary>
        /// <param name="queryExpression"></param>
        /// <param name="keyword"></param>
        /// <param name="fields"></param>
        private void MakeFilterByKeyword(QueryExpression queryExpression, string keyword, params string[] fields)
        {
            FilterExpression filter = new FilterExpression(LogicalOperator.Or);
            int i = 1;
            if (fields.NotEmpty())
            {
                var mainQFields = fields.Where(n => n.IndexOf('.') < 0).ToList();
                if (mainQFields.NotEmpty())
                {
                    foreach (var column in queryExpression.ColumnSet.Columns)//主表
                    {
                        if (mainQFields.NotEmpty() && !mainQFields.Contains(column))
                        {
                            if (i > mainQFields.Count())
                            {
                                break;
                            }

                            continue;
                        }
                        var attr = this.QueryResolver.AttributeList.Find(x => x.Name.IsCaseInsensitiveEqual(column) && x.EntityName.IsCaseInsensitiveEqual(this.QueryExpression.EntityName));
                        filter.AddCondition(GetCondition(attr, keyword));
                        i++;
                    }
                }
            }
            else
            {
                foreach (var column in queryExpression.ColumnSet.Columns)//主表
                {
                    var attr = this.QueryResolver.AttributeList.Find(x => x.Name.IsCaseInsensitiveEqual(column) && x.EntityName.IsCaseInsensitiveEqual(this.QueryExpression.EntityName));
                    filter.AddCondition(GetCondition(attr, keyword));
                    i++;
                }
            }
            if (i > 1)
            {
                queryExpression.Criteria.AddFilter(filter);
            }
            if (fields.NotEmpty() && i > fields.Count())//如果在主实体已匹配完则返回
            {
                return;
            }
            if (queryExpression.LinkEntities.NotEmpty())
            {
                foreach (var le in queryExpression.LinkEntities)//关联表
                {
                    var leColumns = queryExpression.GetAllColumns(le, this.QueryResolver.AttributeList);
                    var linkQFields = leColumns.Where(n => n.IndexOf('.') > 0 && n.Split('.')[0].IsCaseInsensitiveEqual(le.EntityAlias)).ToList();//.Select(n => n.Split('.')[1]).ToArray();
                    if (linkQFields.IsEmpty())
                    {
                        continue;
                    }

                    foreach (var column in linkQFields)
                    {
                        var field = column.Split('.')[1];
                        var attr = this.QueryResolver.AttributeList.Find(x => x.Name.IsCaseInsensitiveEqual(field) && x.EntityName.IsCaseInsensitiveEqual(le.LinkToEntityName));
                        if (attr == null)
                        {
                            continue;
                        }

                        filter.AddCondition(GetCondition(attr, keyword, le.EntityAlias));
                    }
                    //MakeLinkEntityFilterByKeyword(queryExpression, le, keyword, linkQFields);
                }
            }
        }

        /// <summary>
        /// 根据关键字生成过滤条件
        /// </summary>
        /// <param name="queryExpression"></param>
        /// <param name="linkEntity"></param>
        /// <param name="keyword"></param>
        /// <param name="fields"></param>
        private void MakeLinkEntityFilterByKeyword(QueryExpression queryExpression, LinkEntity linkEntity, string keyword, params string[] fields)
        {
            if (linkEntity.LinkCriteria == null || (linkEntity.LinkCriteria.Conditions.IsEmpty() && linkEntity.LinkCriteria.Filters.IsEmpty()))
            {
                linkEntity.LinkCriteria = new FilterExpression(this.QueryExpression.Criteria != null && this.QueryExpression.LinkEntities.IndexOf(linkEntity) == 0 ? this.QueryExpression.Criteria.FilterOperator : LogicalOperator.Or);
            }
            FilterExpression linkFilter = new FilterExpression(LogicalOperator.Or);
            int i = 0;
            foreach (var column in linkEntity.Columns.Columns)
            {
                if (fields.NotEmpty() && !fields.Contains(column))
                {
                    continue;
                }
                var attr = this.QueryResolver.AttributeList.Find(x => x.Name.IsCaseInsensitiveEqual(column) && x.EntityName.IsCaseInsensitiveEqual(linkEntity.LinkToEntityName));
                linkFilter.AddCondition(GetCondition(attr, keyword));
                i++;
            }
            if (i > 0)
            {
                linkEntity.LinkCriteria.AddFilter(linkFilter);
            }
            if (linkEntity.LinkEntities.Count > 0)
            {
                foreach (var le in queryExpression.LinkEntities)
                {
                    MakeLinkEntityFilterByKeyword(queryExpression, le, keyword, fields);
                }
            }
        }

        #endregion conditions

        #region metadata

        /// <summary>
        /// 获取元数据
        /// </summary>
        /// <param name="fetch"></param>
        public void GetMetaDatas(FetchDescriptor fetch)
        {
            if (fetch.Query == null)
            {
                ToQueryExpression(fetch.FetchConfig);
            }
            else
            {
                this.QueryExpression = fetch.Query;
            }
            this.QueryExpression.PageInfo = new PagingInfo() { PageNumber = fetch.Page, PageSize = fetch.PageSize };
            this.QueryResolver = _queryResolverFactory.Get(QueryExpression);
            GetNonePermissionFields();
        }

        public void GetMetaDatas(string fetchConfig)
        {
            ToQueryExpression(fetchConfig);
            this.QueryResolver = _queryResolverFactory.Get(QueryExpression);
            GetNonePermissionFields();
        }

        private void GetNonePermissionFields()
        {
            var securityFields = QueryResolver.AttributeList.Where(n => n.AuthorizationEnabled)?.Select(x => x.AttributeId)?.ToList();
            //无权限的字段
            if (securityFields.NotEmpty())
            {
                var noneReadFields = securityFields.NotEmpty() ? _systemUserPermissionService.GetNoneReadFields(User.SystemUserId, securityFields.ToList()) : null;
                NonePermissionFields = QueryResolver.AttributeList.Where(x => noneReadFields.Contains(x.AttributeId))?.ToList();
            }
        }

        #endregion metadata

        #region query

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="query">查询表达式</param>
        /// <param name="wrapOptionName">是否包含选项名称</param>
        /// <returns></returns>
        public PagedList<dynamic> Execute(QueryExpression query, bool wrapOptionName = false)
        {
            this.QueryExpression = query;
            if (this.User != null && !this.User.IsSuperAdmin && this.User.Roles.NotEmpty())
            {
                var roles = this.User.Roles.Select(r => r.RoleId);
                var entities = this.QueryExpression.GetAllEntityNames();
                var entIds = _entityFinder.FindByNames(entities.ToArray()).Select(n => n.EntityId);
                //this.User.EntityPermissions = _roleEntityPermissionsService.GetPermissions(entIds, roles, AccessRightValue.Read);
                this.User.RoleObjectAccessEntityPermission = _roleObjectAccessEntityPermissionService.GetPermissions(entIds, roles, AccessRightValue.Read);
            }
            this.QueryResolver.Init(query);
            if (this.QueryExpression.PageInfo == null || this.QueryExpression.PageInfo.PageSize < 1)
            {
                PagedList<dynamic> result = new PagedList<dynamic>();
                result.Items = this.QueryResolver.Query();
                result.TotalItems = result.Items.Count;
                if (wrapOptionName)
                {
                    result.Items = WrapOptionName(result.Items);
                }
                return result;
            }
            else
            {
                var result = this.QueryResolver.QueryPaged();
                if (wrapOptionName)
                {
                    result.Items = WrapOptionName(result.Items);
                }
                return result;
            }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="fetch">查询参数</param>
        /// <param name="wrapOptionName">是否包含选项名称</param>
        /// <returns></returns>
        public PagedList<dynamic> Execute(FetchDescriptor fetch, bool wrapOptionName = false)
        {
            //bool hasChange = false;
            GetMetaDatas(fetch);
            if (fetch.User != null)
            {
                if (!fetch.User.IsSuperAdmin && fetch.User.Roles.IsEmpty())
                {
                    fetch.User.Roles = _systemUserRolesService.Query(n => n.Where(f => f.SystemUserId == fetch.User.SystemUserId));
                }
                this.User.Roles = fetch.User.Roles;
            }

            if (fetch.User != null && !fetch.User.IsSuperAdmin && fetch.User.Roles.NotEmpty())
            {
                var roles = fetch.User.Roles.Select(r => r.RoleId);
                var entities = this.QueryExpression.GetAllEntityNames();
                var entIds = _entityFinder.FindByNames(entities.ToArray()).Select(n => n.EntityId);
                fetch.User.RoleObjectAccessEntityPermission = _roleObjectAccessEntityPermissionService.GetPermissions(entIds, roles, AccessRightValue.Read);
                this.User.RoleObjectAccessEntityPermission = fetch.User.RoleObjectAccessEntityPermission;
            }
            //this.User.UserName = "test";
            //this.SetRetrieveConditions(this.User, this.QueryExpression);

            if (fetch.Keyword.IsNotEmpty())
            {
                if (this.QueryExpression.Criteria == null || (this.QueryExpression.Criteria.Conditions.IsEmpty() && this.QueryExpression.Criteria.Filters.IsEmpty()))
                {
                    this.QueryExpression.Criteria = new FilterExpression(LogicalOperator.And);
                }
                else
                {
                    this.QueryExpression.Criteria.FilterOperator = LogicalOperator.And;
                }
                if (fetch.Field.IsNotEmpty())
                {
                    MakeFilterByKeyword(this.QueryExpression, fetch.Keyword, fetch.Field);
                }
                else
                {
                    MakeFilterByKeyword(this.QueryExpression, fetch.Keyword);
                }
                //hasChange = true;
            }
            //过滤
            if (fetch.Filter != null && (fetch.Filter.Filters.Count > 0 || fetch.Filter.Conditions.Count > 0))
            {
                this.QueryExpression.Criteria.AddFilter(fetch.Filter);
                //hasChange = true;
            }
            //排序
            if (fetch.Sort != null)
            {
                //重置排序
                this.QueryExpression.Orders.Clear();
                this.QueryExpression.AddOrder(fetch.Sort.Name, fetch.Sort.SortAscending ? OrderType.Ascending : OrderType.Descending);
                //hasChange = true;
            }
            //无指定排序时，默认第一列
            if (this.QueryExpression.Orders.IsEmpty())
            {
                if (this.QueryResolver.AttributeList.Exists(n => n.Name.IsCaseInsensitiveEqual("createdon")))
                {
                    this.QueryExpression.AddOrder("createdon", OrderType.Descending);
                }
                else
                {
                    this.QueryExpression.AddOrder(this.QueryExpression.ColumnSet.Columns.First(), OrderType.Descending);
                }
                //hasChange = true;
            }

            PagedList<dynamic> result = new PagedList<dynamic>();
            if (fetch.GetAll)
            {
                var data = this.QueryResolver.Query(noneReadFields: NonePermissionFields);
                result.Items = data;
            }
            else
            {
                result = this.QueryResolver.QueryPaged(noneReadFields: NonePermissionFields);
            }
            if (wrapOptionName)
            {
                result.Items = WrapOptionName(result.Items);
            }

            return result;
        }

        //private QueryExpression SetRetrieveConditions(CurrentUser user, QueryExpression request, Schema.Domain.Entity entityMetadata = null)
        //{
        //    if (entityMetadata == null) entityMetadata = new EntityService(user).FindByName(request.EntityName);
        //    if (user.EntityPermissions.IsEmpty())
        //    {
        //        var roles = user.Roles.Select(r => r.RoleId);
        //        //user.EntityPermissions = new RoleEntityPermissionsService(user).Query(n => n.Where(f => f.RoleId.In(roles) && f.AccessRight == AccessRightValue.Read));
        //        var entities = this.QueryExpression.GetAllEntityName();
        //        var _entityService = new EntityService(this.User);
        //        var entIds = _entityService.FindByNames(entities.ToArray()).Select(n => n.EntityId);
        //        user.EntityPermissions = new RoleEntityPermissionsService(this.User).GetPermissions(entIds, roles, AccessRightValue.Read);
        //    }
        //    //filters
        //    if (entityMetadata.IsAuthorization && this.User != null && !this.User.IsSuperAdmin)
        //    {
        //        if (this.User.EntityPermissions.IsEmpty())
        //        {
        //            throw new XmsException(string.Format(Loc["security_noentitypermission"], entityMetadata.LocalizedName, Loc["security_read"]));
        //        }
        //        var a = this.User.EntityPermissions.Where(n => n.EntityId == entityMetadata.EntityId).OrderByDescending(n => n.DepthMask).ToList();
        //        var prv = a.NotEmpty() ? a.First() : null;
        //        if (prv == null)
        //        {
        //            throw new XmsException(string.Format(Loc["security_noentitypermission"], entityMetadata.LocalizedName, Loc["security_read"]));
        //        }
        //        //none
        //        else if (prv.DepthMask == EntityPermissionDepth.None)
        //        {
        //            throw new XmsException(string.Format(Loc["security_noentitypermission"], entityMetadata.LocalizedName, Loc["security_read"]));
        //        }
        //        if (request.Criteria == null || (request.Criteria.Conditions.IsEmpty() && request.Criteria.Filters.IsEmpty()))
        //        {
        //            request.Criteria = new FilterExpression(LogicalOperator.And);
        //        }
        //        //basic
        //        if (prv != null && entityMetadata.EntityMask == EntityMaskEnum.User && prv.DepthMask == Core.EntityPermissionDepth.Self)
        //        {
        //            request.Criteria.AddCondition("OwnerId", ConditionOperator.Equal, this.User.SystemUserId);
        //            //request.Criteria.AddCondition("OwnerId", ConditionOperator.In, "SELECT ObjectId FROM PrincipalObjectAccess WHERE PrincipalId='"+ this.User.SystemUserId + "'");
        //        }
        //        //local
        //        else if (prv != null && entityMetadata.EntityMask == EntityMaskEnum.User && prv.DepthMask == Core.EntityPermissionDepth.BusinessUnit)
        //        {
        //            request.Criteria.AddCondition("OwningBusinessUnit", ConditionOperator.Equal, this.User.BusinessUnitId);
        //        }
        //        //deep
        //        else if (prv != null && entityMetadata.EntityMask == EntityMaskEnum.User && prv.DepthMask == Core.EntityPermissionDepth.BusinessUnitAndChild)
        //        {
        //            var businessUnitIds = new Org.BusinessUnitService(this.User).Query(n => n.Where(f => f.BusinessUnitId == user.BusinessUnitId || f.ParentBusinessUnitId == user.BusinessUnitId));
        //            request.Criteria.AddCondition("OwningBusinessUnit", ConditionOperator.In, businessUnitIds.Select(f => f.BusinessUnitId as object).ToArray());
        //        }
        //        //full
        //        else if (prv != null && prv.DepthMask == EntityPermissionDepth.Organization)
        //        {
        //        }
        //    }
        //    return request;
        //}

        #endregion query

        #region utilities

        /// <summary>
        /// 为数据添加选项名称
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<dynamic> WrapOptionName(IEnumerable<dynamic> data)
        {
            var attributes = this.QueryResolver.AttributeList.Where(n => n.TypeIsBit() || n.TypeIsState() || n.TypeIsPickList()).ToList();
            if (attributes.IsEmpty())
            {
                return data.ToList();
            }
            foreach (var attr in attributes)
            {
                if (attr.TypeIsPickList() || attr.TypeIsStatus())
                {
                    foreach (var d in data)
                    {
                        var _this = d as IDictionary<string, object>;
                        var line = _this.ToList();
                        var columnName = attr.Name.ToLower();
                        if (attr.EntityId != this.MainEntity.EntityId)
                        {
                            var le = this.QueryExpression.FindLinkEntityByName(attr.EntityName);//.LinkEntities.Find(n=>n.LinkToEntityName.IsCaseInsensitiveEqual(attr.EntityName));
                            if (le != null)
                            {
                                columnName = le.EntityAlias + "." + columnName;
                            }
                        }
                        if (_this.ContainsKey(columnName + "name"))
                        {
                            break;
                        }
                        var kv = line.Find(n => n.Key.IsCaseInsensitiveEqual(columnName));
                        if (kv.Value != null)
                        {
                            var o = attr.OptionSet.Items.Find(n => n.Value == int.Parse(kv.Value.ToString()));
                            _this[columnName + "name"] = o != null ? o.Name : string.Empty;
                        }
                        else
                        {
                            _this[columnName + "name"] = string.Empty;
                        }
                    }
                }
                else if (attr.TypeIsState() || attr.TypeIsBit())
                {
                    foreach (var d in data)
                    {
                        var _this = d as IDictionary<string, object>;
                        var line = _this.ToList();
                        var columnName = attr.Name.ToLower();
                        if (attr.EntityId != this.MainEntity.EntityId)
                        {
                            var le = this.QueryExpression.FindLinkEntityByName(attr.EntityName);//.LinkEntities.Find(n => n.LinkToEntityName.IsCaseInsensitiveEqual(attr.EntityName));
                            if (le != null)
                            {
                                columnName = le.EntityAlias + "." + columnName;
                            }
                        }
                        if (_this.ContainsKey(columnName + "name"))
                        {
                            break;
                        }
                        var kv = line.Find(n => n.Key.IsCaseInsensitiveEqual(columnName));
                        if (kv.Value != null)
                        {
                            var bVal = kv.Value.ToString().IsInteger() ? (int.Parse(kv.Value.ToString())) : (bool.Parse(kv.Value.ToString()) ? 1 : 0);
                            var o = attr.PickLists.Find(n => n.Value == bVal);
                            _this[columnName + "name"] = o != null ? o.Name : string.Empty;
                        }
                        else
                        {
                            _this[columnName + "name"] = string.Empty;
                        }
                    }
                }
            }
            return data.ToList();
        }

        private QueryExpression ToQueryExpression(string fetchConfig)
        {
            this.QueryExpression = this.QueryExpression.DeserializeFromJson(fetchConfig);
            return this.QueryExpression;
        }

        #endregion utilities
    }

    public class FetchDescriptor
    {
        public int Page { get; set; }
        public int PageSize { get; set; }

        public bool GetAll { get; set; }

        public string Field { get; set; }
        private string _keyword;

        public string Keyword
        {
            get { return _keyword; }
            set
            {
                _keyword = value.TrimSafe();
            }
        }

        public QueryExpression Query { get; set; }
        public string FetchConfig { get; set; }
        public QueryColumnSortInfo Sort { get; set; }
        public FilterExpression Filter { get; set; }
        public ICurrentUser User { get; set; }
    }
}