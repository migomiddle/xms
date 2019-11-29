using System;
using Xms.Infrastructure.Utility;

namespace Xms.Web.Framework.Infrastructure
{
    public static class Arguments
    {
        public static bool HasValue(params Guid[] value)
        {
            if (value == null || value.Length == 0)
            {
                return false;
            }
            var result = true;
            foreach (var item in value)
            {
                if (item.IsEmpty())
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
    }
}