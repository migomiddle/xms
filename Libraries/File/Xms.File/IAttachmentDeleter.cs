using System;
using System.Collections.Generic;

namespace Xms.File
{
    public interface IAttachmentDeleter
    {
        bool DeleteById(Guid id);

        bool DeleteById(Guid entityId, Guid objectId);

        bool DeleteById(List<Guid> ids);
    }
}