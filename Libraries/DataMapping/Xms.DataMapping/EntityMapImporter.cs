using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Xms.Context;
using Xms.DataMapping.Abstractions;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.DataMapping
{
    /// <summary>
    /// 实体映射导入服务
    /// </summary>
    [SolutionImportNode("entitymaps")]
    public class EntityMapImporter : ISolutionComponentImporter<EntityMapXmlInfo>
    {
        private readonly IEntityMapCreater _entityMapCreater;
        private readonly IEntityMapUpdater _entityMapUpdater;
        private readonly IEntityMapFinder _entityMapFinder;
        private readonly IAttributeMapCreater _attributeMapCreater;
        private readonly IAttributeMapDeleter _attributeMapDeleter;
        private readonly IAppContext _appContext;

        public EntityMapImporter(IAppContext appContext
            , IEntityMapCreater entityMapCreater
            , IEntityMapUpdater entityMapUpdater
            , IEntityMapFinder entityMapFinder
            , IAttributeMapCreater attributeMapCreater
            , IAttributeMapDeleter attributeMapDeleter)
        {
            _appContext = appContext;
            _entityMapCreater = entityMapCreater;
            _entityMapUpdater = entityMapUpdater;
            _entityMapFinder = entityMapFinder;
            _attributeMapCreater = attributeMapCreater;
            _attributeMapDeleter = attributeMapDeleter;
        }

        public bool Import(Guid solutionId, IList<EntityMapXmlInfo> entityMaps)
        {
            if (entityMaps.NotEmpty())
            {
                foreach (var item in entityMaps)
                {
                    var entity = _entityMapFinder.FindById(item.EntityMapId);
                    if (entity != null)
                    {
                        _entityMapUpdater.Update(item);
                        if (item.AttributeMaps.NotEmpty())
                        {
                            _attributeMapDeleter.DeleteByParentId(entity.EntityMapId);
                            _attributeMapCreater.CreateMany(item.AttributeMaps);
                        }
                    }
                    else
                    {
                        item.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        item.SolutionId = solutionId;
                        item.ComponentState = 0;
                        item.CreatedOn = DateTime.Now;
                        _entityMapCreater.Create(item);
                    }
                }
            }
            return true;
        }
    }

    public class EntityMapXmlInfo : Domain.EntityMap
    {
        [XmlIgnore]
        public new MapType MapType { get; set; }

        [XmlAttribute("MapType")]
        public int MapTypeInt
        {
            get { return (int)MapType; }
            set { MapType = (MapType)value; }
        }
    }
}