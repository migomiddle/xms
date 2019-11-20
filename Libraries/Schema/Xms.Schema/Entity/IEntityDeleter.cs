using System;

namespace Xms.Schema.Entity
{
    public interface IEntityDeleter
    {
        bool DeleteById(bool deleteTable = true, params Guid[] id);
    }
}