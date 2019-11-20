using System;
using System.Collections.Generic;
using Xms.Infrastructure.Utility;
using Xms.Schema.Domain;

namespace Xms.Schema.RelationShip
{
    /// <summary>
    /// 关系导入服务
    /// </summary>
    public class RelationShipImporter
    {
        private readonly IRelationShipCreater _relationShipCreater;
        private readonly IRelationShipUpdater _relationShipUpdater;
        private readonly IRelationShipFinder _relationShipFinder;

        public RelationShipImporter(IRelationShipCreater relationShipCreater
            , IRelationShipUpdater relationShipUpdater
            , IRelationShipFinder relationShipFinder)
        {
            _relationShipCreater = relationShipCreater;
            _relationShipUpdater = relationShipUpdater;
            _relationShipFinder = relationShipFinder;
        }

        private bool Import(Guid solutionId, List<RelationShipXmlInfo> relationShips)
        {
            if (relationShips.NotEmpty())
            {
                foreach (var item in relationShips)
                {
                    var entity = _relationShipFinder.FindById(item.RelationshipId);
                    if (entity != null)
                    {
                        _relationShipUpdater.Update(item);
                    }
                    else
                    {
                        _relationShipCreater.Create(item);
                    }
                }
            }
            return true;
        }
    }
}