using System;

namespace Xms.Schema.RelationShip
{
    public interface IRelationShipDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}