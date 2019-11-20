using System.Collections.Generic;
using Xms.Context;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Extensions;
using Xms.Sdk.Query;

namespace Xms.Sdk.Data
{
    /// <summary>
    /// 查询表达式解析器
    /// </summary>
    public class QueryByAttributeResolver : IQueryResolver
    {
        private readonly DataRepositoryBase<dynamic> _repository;
        private readonly IQueryMetadataFinder _queryMetadataFinder;
        private readonly IDbContext _dbContext;

        public QueryByAttributeResolver(IAppContext appContext
            , IDbContext dbContext
            , IQueryMetadataFinder queryMetadataFinder)
        {
            this.User = appContext.GetFeature<ICurrentUser>();
            _dbContext = dbContext;
            _repository = new DataRepositoryBase<dynamic>(_dbContext);
            _queryMetadataFinder = queryMetadataFinder;
        }

        public IQueryResolver Init(QueryBase query)
        {
            _queryByAttribute = query as QueryByAttribute;
            if (query != null && query.EntityName.IsNotEmpty())
            {
                var metadatas = _queryMetadataFinder.GetAll(query);
                EntityList = metadatas.entities;
                AttributeList = metadatas.attributes;
                RelationShipList = metadatas.relationShips;
            }
            return this;
        }

        public ICurrentUser User { get; set; }

        private QueryByAttribute _queryByAttribute = new QueryByAttribute();

        public QueryBase QueryObject
        {
            get { return _queryByAttribute; }
            set { _queryByAttribute = value as QueryByAttribute; }
        }

        private List<Schema.Domain.Entity> _entityList;

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

        private List<Schema.Domain.Attribute> _attributeList;

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

        public List<Schema.Domain.RelationShip> RelationShipList { get; set; }

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

        public string AliasJoiner
        {
            get; set;
        }

        public List<AttributeAlias> AttributeAliasList
        {
            get; set;
        }

        #region 查询数据

        public PagedList<dynamic> QueryPaged(bool ignorePermissions = false, List<Schema.Domain.Attribute> noneReadFields = null)
        {
            var result = _repository.ExecuteQueryPaged(this._queryByAttribute.PageInfo.PageNumber, this._queryByAttribute.PageInfo.PageSize, ToSqlString(ignorePermissions: ignorePermissions, noneReadFields: noneReadFields), this.Parameters.Args.ToArray());
            return result;
        }

        public List<dynamic> Query(bool ignorePermissions = false, List<Schema.Domain.Attribute> noneReadFields = null)
        {
            var result = _repository.ExecuteQuery(ToSqlString(ignorePermissions: ignorePermissions, noneReadFields: noneReadFields), this.Parameters.Args.ToArray());
            return result;
        }

        public dynamic Find(bool ignorePermissions = false, List<Schema.Domain.Attribute> noneReadFields = null)
        {
            var result = _repository.Find(ToSqlString(ignorePermissions: ignorePermissions, noneReadFields: noneReadFields), this.Parameters.Args.ToArray());
            return result;
        }

        #endregion 查询数据

        #region 生成SQL

        public string ToSqlString(bool includeNameField = true, bool ignorePermissions = false, List<Schema.Domain.Attribute> noneReadFields = null)
        {
            if (!this._queryByAttribute.ColumnSet.AllColumns && this._queryByAttribute.ColumnSet.Columns.Find(x => x.IsCaseInsensitiveEqual(MainEntity.Name + "Id")) == null)
            {
                this._queryByAttribute.ColumnSet.AddColumn(MainEntity.Name + "Id");
            }
            List<string> columns = new List<string>();
            List<string> filters = new List<string>();
            List<string> orders = new List<string>();
            //columns
            if (_queryByAttribute.ColumnSet.AllColumns)
            {
                columns.Add("*");
            }
            else
            {
                foreach (var item in _queryByAttribute.ColumnSet.Columns)
                {
                    columns.Add(item);
                }
            }
            //filters
            int i = 0;
            foreach (var item in _queryByAttribute.Attributes)
            {
                filters.Add(string.Format("{0}.[{1}]={2}", MainEntity.Name, item, "@" + i));
                i++;
            }
            Parameters.Args = (_queryByAttribute.Values);

            //orders
            foreach (var ord in _queryByAttribute.Orders)
            {
                orders.Add(MainEntity.Name + "." + ord.AttributeName + " " + (ord.OrderType == OrderType.Descending ? " DESC" : ""));
            }
            return string.Format("SELECT {0} FROM {1}View AS {1} {2} {3}", string.Join(",", columns).ToLower(), MainEntity.Name
                , filters.NotEmpty() ? " WHERE " + string.Join(" AND ", filters) : string.Empty
                , orders.NotEmpty() ? " ORDER BY " + string.Join(",", orders) : string.Empty);
        }

        #endregion 生成SQL
    }
}