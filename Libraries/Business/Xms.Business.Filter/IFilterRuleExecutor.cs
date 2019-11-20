using Xms.Core;
using Xms.Core.Data;

namespace Xms.Business.Filter
{
    public interface IFilterRuleExecutor
    {
        void Execute(OperationTypeEnum op, Entity data, Schema.Domain.Entity entityMetadata);
    }
}