using Xms.Core.Context;

namespace Xms.Data.Provider
{
    public class UpdateContextBuilder
    {
        public static UpdateContext<T> Build<T>() where T : class
        {
            return new UpdateContext<T>(new ExpressionParser((entityType, name) => { return PocoHelper.FormatColumn(entityType, name); }));
        }
    }
}