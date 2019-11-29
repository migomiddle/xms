namespace Xms.RibbonButton
{
    public class RibbonButtonCache
    {
        public static string BuildKey(Domain.RibbonButton entity)
        {
            return entity.EntityId + "/" + entity.RibbonButtonId + "/";
        }
    }
}