using System;

namespace Xms.DataMapping
{
    public interface IAttributeMapDeleter
    {
        bool DeleteById(params Guid[] id);

        bool DeleteByParentId(Guid entityMapId);
    }
}