using System.Collections.Generic;
using Xms.Core.Data;

namespace Xms.Business.DuplicateValidator
{
    public interface IDuplicateRuleExecutor
    {
        IEnumerable<DuplicateRuleHitResult> ExecuteCore(Entity data, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas);
    }
}