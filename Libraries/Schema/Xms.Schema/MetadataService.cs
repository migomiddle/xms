using System.Collections.Generic;
using Xms.Schema.Attribute;
using Xms.Schema.Data;
using Xms.Schema.RelationShip;

namespace Xms.Schema
{
    /// <summary>
    /// 数据库架构服务
    /// </summary>
    public class MetadataService : IMetadataService
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDefaultAttributeProvider _defaultAttributeProvider;
        private readonly IRelationShipFinder _relationShipFinder;

        public MetadataService(IMetadataProvider metadataProvider
            , IAttributeFinder attributeFinder
            , IDefaultAttributeProvider defaultAttributeProvider
            , IRelationShipFinder relationShipFinder)
        {
            _metadataProvider = metadataProvider;
            _attributeFinder = attributeFinder;
            _defaultAttributeProvider = defaultAttributeProvider;
            _relationShipFinder = relationShipFinder;
        }

        public void AddColumn(Domain.Attribute attr)
        {
            _metadataProvider.AddColumn(attr);
        }

        public void AlterColumn(Domain.Attribute attr)
        {
            _metadataProvider.AlterColumn(attr);
        }

        public void CreateView(Domain.Entity entity)
        {
            var defaultAttributes = _defaultAttributeProvider.GetSysAttributes(entity);
            _metadataProvider.AlterView(entity, defaultAttributes, _defaultAttributeProvider.GetSysAttributeRelationShips(entity, defaultAttributes));
        }

        public void AlterView(Domain.Entity entity)
        {
            //不从缓存获取，这里要读取未提交的记录
            var attributes = _attributeFinder.Query(x => x.Where(f => f.EntityId == entity.EntityId));
            var relationShips = _relationShipFinder.Query(x => x.Where(f => f.ReferencingEntityId == entity.EntityId));
            _metadataProvider.AlterView(entity, attributes, relationShips);
        }

        public void CreateTable(Domain.Entity entity, List<Domain.Attribute> defaultAttributes)
        {
            _metadataProvider.CreateTable(entity, defaultAttributes);
        }

        public void DropColumn(Domain.Attribute attr)
        {
            _metadataProvider.DropColumn(attr);
        }

        public void DropTable(Domain.Entity entity)
        {
            _metadataProvider.DropTable(entity);
        }

        public void DropView(Domain.Entity entity)
        {
            _metadataProvider.DropView(entity);
        }
    }
}