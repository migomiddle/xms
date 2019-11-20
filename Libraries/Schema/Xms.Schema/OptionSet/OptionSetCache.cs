namespace Xms.Schema.OptionSet
{
    public class OptionSetCache
    {
        public static string BuildKey(Domain.OptionSet entity)
        {
            return entity.OptionSetId.ToString();
        }
    }
}