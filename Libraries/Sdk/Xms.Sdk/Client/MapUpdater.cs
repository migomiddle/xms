using System;
using Xms.Core.Data;
using Xms.DataMapping;
using Xms.DataMapping.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Sdk.Data;
using Xms.Sdk.Extensions;

namespace Xms.Sdk.Client
{
    /// <summary>
    /// 实体转换反写源记录值服务
    /// </summary>
    public class MapUpdater : IMapUpdater
    {
        private readonly IFieldValueUpdater _fieldValueUpdater;
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IEntityMapFinder _entityMapFinder;
        private readonly IAttributeMapFinder _attributeMapFinder;

        public MapUpdater(
            IFieldValueUpdater fieldValueUpdater
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IEntityMapFinder entityMapFinder
            , IAttributeMapFinder attributeMapFinder
            )
        {
            _fieldValueUpdater = fieldValueUpdater;
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _entityMapFinder = entityMapFinder;
            _attributeMapFinder = attributeMapFinder;
        }

        public bool Update(Schema.Domain.Entity targetEntityMetadata, Entity targetRecord, bool onDelete = false)
        {
            var entityMaps = _entityMapFinder.Query(n => n.Where(f => f.TargetEntityId == targetEntityMetadata.EntityId && (f.MapType == MapType.Control || f.MapType == MapType.ForceControl)));
            if (entityMaps.IsEmpty())
            {
                return true;
            }
            if (targetRecord.IsEmpty())
            {
                return false;
            }
            Guid headSourceId = Guid.Empty;
            var emptyGuid = Guid.Empty;
            foreach (var em in entityMaps)
            {
                var attributeMaps = _attributeMapFinder.Query(n => n.Where(f => f.EntityMapId == em.EntityMapId && f.RemainAttributeId != emptyGuid));
                //单据头
                if (attributeMaps.NotEmpty())
                {
                    var sourceAttributesMeta = _attributeFinder.FindByEntityId(em.SourceEntityId);
                    var targetEntityMetas = _entityFinder.FindById(em.TargetEntityId);
                    var targetAttributesMeta = _attributeFinder.FindByEntityId(em.TargetEntityId);
                    //引用源单据的字段
                    var refSourceAttr = targetAttributesMeta.Find(n => n.ReferencedEntityId.HasValue && n.ReferencedEntityId.Value == em.SourceEntityId);
                    //更新单据头相关字段
                    _fieldValueUpdater.UpdateControlMap(em, attributeMaps, targetRecord.GetGuidValue(refSourceAttr.Name), refSourceAttr, sourceAttributesMeta, targetAttributesMeta, onDelete);
                }
                return true;
            }
            return true;
        }
    }
}