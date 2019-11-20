using Xms.Business.DuplicateValidator.Domain;

namespace Xms.Business.DuplicateValidator
{
    public interface IDuplicateRuleCreater
    {
        bool Create(DuplicateRule entity);
    }
}