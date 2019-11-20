using System;
using Xms.Core.Context;

namespace Xms.Schema.RelationShip
{
    public interface IRelationShipUpdater
    {
        bool Update(Func<UpdateContext<Domain.RelationShip>, UpdateContext<Domain.RelationShip>> context);

        bool Update(Domain.RelationShip entity);
    }
}