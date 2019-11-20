namespace Xms.QueryView
{
    public class QueryViewCache
    {
        public static string BuildKey(Domain.QueryView entity)
        {
            return entity.EntityId + "/" + entity.EntityName + "/" + entity.QueryViewId + "/";
        }
    }
}