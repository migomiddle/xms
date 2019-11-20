using System.Collections.Generic;
using Xms.Schema.Domain;
using Xms.Sdk.Abstractions.Query;

namespace Xms.Sdk.Extensions
{
    public interface IQueryMetadataFinder
    {
        (List<Entity> entities, List<Attribute> attributes, List<RelationShip> relationShips) GetAll(QueryBase query);

        List<Attribute> GetAttributes(QueryBase query);

        List<Entity> GetEntities(QueryBase query);

        List<RelationShip> GetRelationShips(QueryBase query);
    }
}