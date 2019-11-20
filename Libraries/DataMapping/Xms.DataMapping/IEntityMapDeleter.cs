using System;

namespace Xms.DataMapping
{
    public interface IEntityMapDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}