using System;

namespace Xms.Schema.Attribute
{
    public interface IAttributeDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}