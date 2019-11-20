using System;

namespace Xms.Schema.StringMap
{
    public interface IStringMapDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}