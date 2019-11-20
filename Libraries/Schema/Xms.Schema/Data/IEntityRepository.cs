using System;
using System.Collections.Generic;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Schema.Abstractions;

namespace Xms.Schema.Data
{
    public interface IEntityRepository : IRepository<Domain.Entity>
    {
        bool Create(Domain.Entity entity, List<Domain.Attribute> defaultAttributes, List<Domain.RelationShip> defaultRelationShips);

        bool DeleteById(Guid id, bool dropTable);

        IEnumerable<Domain.Entity> QueryRelated(Guid entityid, RelationShipType type, int cascadeLinkMask = -1);

        PagedList<Domain.Entity> QueryPaged(QueryDescriptor<Domain.Entity> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}