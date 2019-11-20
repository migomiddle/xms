using System;
using System.Collections.Generic;
using Xms.Core.Context;
using Xms.Schema.Abstractions;

namespace Xms.Schema.Entity
{
    public interface IEntityFinder
    {
        bool Exists(string name);

        Domain.Entity FindById(Guid id);

        Domain.Entity FindByName(string name);

        List<Domain.Entity> FindByNames(params string[] name);

        List<Domain.Entity> FindAll();

        List<Domain.Entity> Query(Func<QueryDescriptor<Domain.Entity>, QueryDescriptor<Domain.Entity>> container);

        PagedList<Domain.Entity> QueryPaged(Func<QueryDescriptor<Domain.Entity>, QueryDescriptor<Domain.Entity>> container);

        PagedList<Domain.Entity> QueryPaged(Func<QueryDescriptor<Domain.Entity>, QueryDescriptor<Domain.Entity>> container, Guid solutionId, bool existInSolution);

        List<Domain.Entity> QueryRelated(Guid entityid, RelationShipType type, int cascadeLinkMask = -1);
    }
}