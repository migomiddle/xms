using System.Collections.Generic;

namespace Xms.Schema.RelationShip
{
    public interface IRelationShipCreater
    {
        bool Create(Domain.RelationShip entity);

        bool CreateMany(List<Domain.RelationShip> entities);
    }
}