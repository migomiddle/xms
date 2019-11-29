using System;
using Xms.Core.Context;
using Xms.Core.Data;

namespace Xms.File
{
    public interface IAttachmentFinder
    {
        Entity FindById(Guid id);

        PagedList<Entity> QueryPaged(int page, int pageSize, Guid entityId, Guid objectId);
    }
}