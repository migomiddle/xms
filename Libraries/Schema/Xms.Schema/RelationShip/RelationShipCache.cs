namespace Xms.Schema.RelationShip
{
    public class RelationShipCache
    {
        public static string BuildKey(Domain.RelationShip entity)
        {
            return entity.ReferencingEntityId + "/" + entity.ReferencedEntityId + "/" + entity.RelationshipId + "/" + entity.Name + "/";
        }
    }
}