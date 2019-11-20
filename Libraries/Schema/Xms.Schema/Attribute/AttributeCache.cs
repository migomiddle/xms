namespace Xms.Schema.Attribute
{
    public class AttributeCache
    {
        public static string BuildKey(Domain.Attribute entity)
        {
            return entity.EntityId + "/" + entity.EntityName + "/" + entity.AttributeId + "/" + entity.Name + "/";
        }
    }
}