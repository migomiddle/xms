using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Infrastructure.Utility;
using Xms.Schema.Attribute;
using Xms.Schema.Domain;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Schema.OptionSet;
using Xms.Schema.RelationShip;
using Xms.Schema.StringMap;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Extensions
{
    /// <summary>
    /// 查询对象元数据查找
    /// </summary>
    public class QueryMetadataFinder : IQueryMetadataFinder
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IRelationShipFinder _relationShipFinder;
        private readonly IStringMapFinder _stringMapFinder;
        private readonly IOptionSetDetailFinder _optionSetDetailFinder;

        public QueryMetadataFinder(IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IRelationShipFinder relationShipFinder
            , IStringMapFinder stringMapFinder
            , IOptionSetDetailFinder optionSetDetailFinder)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _relationShipFinder = relationShipFinder;
            _stringMapFinder = stringMapFinder;
            _optionSetDetailFinder = optionSetDetailFinder;
        }

        private List<Schema.Domain.Attribute> _attributeList;
        private List<Schema.Domain.Entity> _entityList;
        private List<RelationShip> _relationShipList;

        public (List<Schema.Domain.Entity> entities, List<Schema.Domain.Attribute> attributes, List<RelationShip> relationShips) GetAll(QueryBase query)
        {
            if (query != null && query.EntityName.IsNotEmpty())
            {
                GetMetaData(query, true, true, true);
            }
            return (_entityList, _attributeList, _relationShipList);
        }

        public List<Schema.Domain.Entity> GetEntities(QueryBase query)
        {
            if (query != null && query.EntityName.IsNotEmpty())
            {
                GetMetaData(query, true, false, false);
            }
            return _entityList;
        }

        public List<Schema.Domain.Attribute> GetAttributes(QueryBase query)
        {
            if (query != null && query.EntityName.IsNotEmpty())
            {
                GetMetaData(query, false, true, false);
            }
            return _attributeList;
        }

        public List<Schema.Domain.RelationShip> GetRelationShips(QueryBase query)
        {
            if (query != null && query.EntityName.IsNotEmpty())
            {
                GetMetaData(query, false, false, true);
            }
            return _relationShipList;
        }

        private void GetLinkMetaData(LinkEntity linkEntity, bool getEntities, bool getAttributes, bool getRelationShips)
        {
            //关联实体
            if (getEntities)
            {
                var entity = _entityFinder.FindByName(linkEntity.LinkToEntityName);
                _entityList.Add(entity);
            }
            //关联实体所有字段
            if (getAttributes)
            {
                var leAttributes = _attributeFinder.FindByEntityName(linkEntity.LinkToEntityName);
                if (leAttributes.NotEmpty())
                {
                    if (!linkEntity.Columns.AllColumns && linkEntity.Columns.Columns.NotEmpty())
                    {
                        leAttributes.RemoveAll(x => !linkEntity.Columns.Columns.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase));
                    }
                    _attributeList.AddRange(leAttributes);
                }
            }
            //关系
            if (getRelationShips)
            {
                var rs = _relationShipFinder.Find(x => x.Name == linkEntity.EntityAlias);
                _relationShipList = new List<RelationShip>();
                _relationShipList.Add(rs);

                if (linkEntity.LinkEntities.NotEmpty())
                {
                    foreach (var le in linkEntity.LinkEntities)
                    {
                        GetLinkMetaData(le, getEntities, getAttributes, getRelationShips);
                    }
                }
            }
        }

        /// <summary>
        /// 获取关联的实体及字段元数据
        /// </summary>
        private void GetMetaData(QueryBase query, bool getEntities, bool getAttributes, bool getRelationShips)
        {
            //保存主实体
            if (getEntities)
            {
                _entityList = new List<Schema.Domain.Entity>
                {
                    _entityFinder.FindByName(query.EntityName)
                };
            }
            //获取实体所有字段
            if (getAttributes)
            {
                var entityAttributes = _attributeFinder.FindByEntityName(query.EntityName);
                _attributeList = new List<Schema.Domain.Attribute>();
                if (query is QueryExpression)
                {
                    var queryExp = query as QueryExpression;
                    if (!queryExp.ColumnSet.AllColumns && queryExp.ColumnSet.Columns.NotEmpty())
                    {
                        _attributeList.AddRange(entityAttributes.Where(x => queryExp.ColumnSet.Columns.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase)
                        || (x.IsPrimaryField || x.TypeIsPrimaryKey())));
                    }
                    else
                    {
                        _attributeList = entityAttributes;
                    }
                }
                else if (query is QueryByAttribute)
                {
                    var queryExp = query as QueryByAttribute;
                    if (!queryExp.ColumnSet.AllColumns && queryExp.ColumnSet.Columns.NotEmpty())
                    {
                        _attributeList.AddRange(entityAttributes.Where(x => queryExp.ColumnSet.Columns.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase)
                        || (x.IsPrimaryField || x.TypeIsPrimaryKey())));
                    }
                    else
                    {
                        _attributeList = entityAttributes;
                    }
                }
            }

            //link entities
            if (query is QueryExpression)
            {
                var queryExp = query as QueryExpression;
                foreach (var le in queryExp.LinkEntities)
                {
                    GetLinkMetaData(le, getEntities, getAttributes, getRelationShips);
                }
            }
            if (getAttributes)
            {
                //attribute options
                foreach (var attr in _attributeList)
                {
                    if (attr.TypeIsPickList() || attr.TypeIsStatus())
                    {
                        if (attr.OptionSet == null)
                        {
                            var options = _optionSetDetailFinder.Query(x => x.Where(f => f.OptionSetId == attr.OptionSetId.Value).Sort(s => s.SortAscending(f => f.DisplayOrder)));
                            attr.OptionSet = new OptionSet();
                            attr.OptionSet.Items = options?.ToList();
                        }
                    }
                    else if (attr.TypeIsState() || attr.TypeIsBit())
                    {
                        if (attr.PickLists == null)
                        {
                            attr.PickLists = _stringMapFinder.Query(x => x.Where(f => f.AttributeId == attr.AttributeId).Sort(s => s.SortAscending(f => f.DisplayOrder)))?.ToList();
                        }
                    }
                }
            }
        }
    }
}