using Xms.Core.Context;

namespace Xms.Data.Provider
{
    public class FilterContainerBuilder
    {
        public static FilterContainer<T> Build<T>() where T : class
        {
            return new FilterContainer<T>(new ExpressionParser((entityType, name) => { return PocoHelper.FormatColumn(entityType, name); }));
        }
    }
}