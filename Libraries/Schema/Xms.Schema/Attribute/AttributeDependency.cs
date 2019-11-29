using System;
using Xms.Dependency;
using Xms.Schema.Abstractions;
using Xms.Schema.Data;
using Xms.Schema.Extensions;

namespace Xms.Schema.Attribute
{
    /// <summary>
    /// 字段依赖服务
    /// </summary>
    public class AttributeDependency : IAttributeDependency
    {
        private readonly IAttributeRepository _attributeRepository;
        private readonly IDependencyService _dependencyService;
        private readonly IDependencyBatchBuilder _dependencyBatchBuilder;

        public AttributeDependency(IAttributeRepository attributeRepository
            , IDependencyService dependencyService
            , IDependencyBatchBuilder dependencyBatchBuilder)
        {
            _attributeRepository = attributeRepository;
            _dependencyService = dependencyService;
            _dependencyBatchBuilder = dependencyBatchBuilder;
        }

        public bool Create(Domain.Attribute entity)
        {
            //引用类型
            if (entity.TypeIsLookUp() || entity.TypeIsOwner() || entity.TypeIsCustomer())
            {
                var referenced = _attributeRepository.Find(x => x.EntityId == entity.ReferencedEntityId.Value && x.AttributeTypeName == AttributeTypeIds.PRIMARYKEY);
                //依赖于被引用实体
                _dependencyBatchBuilder.Append(AttributeDefaults.ModuleName, entity.AttributeId, EntityDefaults.ModuleName, entity.ReferencedEntityId.Value)
                .Append(AttributeDefaults.ModuleName, entity.AttributeId, AttributeDefaults.ModuleName, referenced.AttributeId)
                .Save();
            }
            //选项类型
            else if (entity.TypeIsPickList())
            {
                if (entity.OptionSet != null && entity.OptionSet.IsPublic)
                {
                    //依赖于公共选项集
                    _dependencyBatchBuilder.Append(AttributeDefaults.ModuleName, entity.AttributeId, OptionSetDefaults.ModuleName, entity.OptionSetId.Value)
                        .Save();
                }
            }
            return true;
        }

        public bool Update(Domain.Attribute entity)
        {
            //引用类型
            if (entity.TypeIsLookUp() || entity.TypeIsOwner() || entity.TypeIsCustomer())
            {
                var referenced = _attributeRepository.Find(x => x.EntityId == entity.ReferencedEntityId.Value && x.AttributeTypeName == AttributeTypeIds.PRIMARYKEY);
                _dependencyService.Update(AttributeDefaults.ModuleName, entity.AttributeId, EntityDefaults.ModuleName, entity.ReferencedEntityId.Value);
                _dependencyService.Update(AttributeDefaults.ModuleName, entity.AttributeId, AttributeDefaults.ModuleName, referenced.AttributeId);
            }
            //选项类型
            else if (entity.TypeIsPickList())
            {
                if (entity.OptionSet != null && entity.OptionSet.IsPublic)
                {
                    //依赖于公共选项集
                    _dependencyService.Update(AttributeDefaults.ModuleName, entity.AttributeId, OptionSetDefaults.ModuleName, entity.OptionSetId.Value);
                }
            }
            return true;
        }

        public bool Delete(params Guid[] id)
        {
            return _dependencyService.DeleteByDependentId(AttributeDefaults.ModuleName, id); ;
        }
    }
}