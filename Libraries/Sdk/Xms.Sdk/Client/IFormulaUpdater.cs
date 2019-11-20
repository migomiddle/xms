using Xms.Core.Data;

namespace Xms.Sdk.Client
{
    public interface IFormulaUpdater
    {
        bool Update(Schema.Domain.Entity entityMetadata, Entity data);
    }
}