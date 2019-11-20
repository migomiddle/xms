using System;

namespace Xms.Sdk.Client
{
    public interface IDataMapper
    {
        Guid Create(Guid sourceEntityId, Guid targetEntityId, Guid sourceRecordId, bool ignorePermissions = false);
    }
}