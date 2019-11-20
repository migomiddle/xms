using System;

namespace Xms.Business.DuplicateValidator
{
    public interface IDuplicateRuleDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}