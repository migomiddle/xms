using System;

namespace Xms.Form
{
    public interface ISystemFormDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}