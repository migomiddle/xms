using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.DataMapping.Data;
using Xms.DataMapping.Domain;

namespace Xms.DataMapping
{
    /// <summary>
    /// 实体字段映射查找服务
    /// </summary>
    public class AttributeMapFinder : IAttributeMapFinder
    {
        private readonly IAttributeMapRepository _attributeMapRepository;

        public AttributeMapFinder(IAppContext appContext
            , IAttributeMapRepository attributeMapRepository)
        {
            _attributeMapRepository = attributeMapRepository;
        }

        public AttributeMap FindById(Guid id)
        {
            var data = _attributeMapRepository.FindById(id);
            return data;
        }

        public AttributeMap Find(Expression<Func<AttributeMap, bool>> predicate)
        {
            var data = _attributeMapRepository.Find(predicate);
            return data;
        }

        public PagedList<AttributeMap> QueryPaged(Func<QueryDescriptor<AttributeMap>, QueryDescriptor<AttributeMap>> container)
        {
            QueryDescriptor<AttributeMap> q = container(QueryDescriptorBuilder.Build<AttributeMap>());
            var datas = _attributeMapRepository.QueryPaged(q);
            return datas;
        }

        public List<AttributeMap> Query(Func<QueryDescriptor<AttributeMap>, QueryDescriptor<AttributeMap>> container)
        {
            QueryDescriptor<AttributeMap> q = container(QueryDescriptorBuilder.Build<AttributeMap>());
            var datas = _attributeMapRepository.Query(q)?.ToList();
            return datas;
        }
    }
}