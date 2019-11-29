using System;

namespace Xms.Plugin
{
    public interface IEntityPluginDeleter
    {
        bool DeleteById(params Guid[] id);

        bool DeleteByEntityId(Guid entityId);
    }
}