using System.Collections.Generic;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Query;

namespace Xms.Sdk.Data
{
    /// <summary>
    /// 实体数据查询
    /// </summary>
    public class OrganizationDataRetriever : IOrganizationDataRetriever
    {
        private readonly IQueryResolverFactory _queryResolverFactory;

        public OrganizationDataRetriever(
            IQueryResolverFactory queryResolverFactory
            )
        {
            _queryResolverFactory = queryResolverFactory;
        }

        public PagedList<Entity> RetrievePaged(QueryBase request, bool ignorePermissions = false)
        {
            var result = _queryResolverFactory.Get(request).QueryPaged(ignorePermissions);
            var ec = new PagedList<Entity>();
            var items = new List<Entity>();
            foreach (var item in result.Items)
            {
                var entity = new Entity(request.EntityName, item);
                items.Add(entity);
            }
            ec.Items = items;
            ec.ItemsPerPage = result.ItemsPerPage;
            ec.CurrentPage = result.CurrentPage;
            ec.TotalItems = result.TotalItems;
            ec.TotalPages = result.TotalPages;
            return ec;
        }

        public IEnumerable<Entity> RetrieveAll(QueryBase request, bool ignorePermissions = false)
        {
            var result = _queryResolverFactory.Get(request).Query(ignorePermissions);
            var items = new List<Entity>();
            foreach (var item in result)
            {
                var entity = new Entity(request.EntityName, item);
                items.Add(entity);
            }
            return items;
        }

        public Entity Retrieve(QueryBase request, bool ignorePermissions = false)
        {
            var result = _queryResolverFactory.Get(request).Find(ignorePermissions);
            Entity entity;
            if (result != null)
            {
                entity = new Entity(request.EntityName, result);
            }
            else
            {
                entity = new Entity(request.EntityName);
            }

            return entity;
        }
    }
}