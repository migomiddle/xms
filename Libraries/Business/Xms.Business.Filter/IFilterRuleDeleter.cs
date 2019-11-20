using System;

namespace Xms.Business.Filter
{
    public interface IFilterRuleDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}