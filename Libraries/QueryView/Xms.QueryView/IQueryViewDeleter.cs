using System;

namespace Xms.QueryView
{
    public interface IQueryViewDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}