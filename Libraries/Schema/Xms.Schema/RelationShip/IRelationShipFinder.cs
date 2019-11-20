using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;

namespace Xms.Schema.RelationShip
{
    public interface IRelationShipFinder
    {
        Domain.RelationShip Find(Expression<Func<Domain.RelationShip, bool>> predicate);

        Domain.RelationShip FindById(Guid id);

        Domain.RelationShip FindByName(string name);

        List<Domain.RelationShip> FindAll();

        List<Domain.RelationShip> Query(Func<QueryDescriptor<Domain.RelationShip>, QueryDescriptor<Domain.RelationShip>> container);

        List<Domain.RelationShip> QueryByEntityId(Guid? referencingEntityId, Guid? referencedEntityId);

        PagedList<Domain.RelationShip> QueryPaged(Func<QueryDescriptor<Domain.RelationShip>, QueryDescriptor<Domain.RelationShip>> container);

        void WrapLocalizedLabel(Domain.RelationShip entity);

        void WrapLocalizedLabel(IList<Domain.RelationShip> datas);
    }
}