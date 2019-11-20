using Xms.Core.Data;

namespace Xms.Sdk.Client
{
    public interface IMapUpdater
    {
        bool Update(Schema.Domain.Entity targetEntityMetadata, Entity targetRecord, bool onDelete = false);
    }
}