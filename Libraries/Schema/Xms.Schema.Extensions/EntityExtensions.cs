namespace Xms.Schema.Extensions
{
    public static class EntityExtensions
    {
        public static string GetIdName(this Domain.Entity entity)
        {
            return $"{entity.Name}Id";
        }
    }
}