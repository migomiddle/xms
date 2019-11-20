namespace Xms.Schema.Entity
{
    public class EntityCache
    {
        public static string BuildKey(Domain.Entity entity)
        {
            return entity.EntityId + "/" + entity.Name + "/";
        }
    }
}