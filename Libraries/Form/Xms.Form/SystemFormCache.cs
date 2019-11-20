namespace Xms.Form
{
    public class SystemFormCache
    {
        public static string BuildKey(Domain.SystemForm entity)
        {
            return entity.EntityId + "/" + entity.EntityName + "/" + entity.SystemFormId + "/";
        }
    }
}