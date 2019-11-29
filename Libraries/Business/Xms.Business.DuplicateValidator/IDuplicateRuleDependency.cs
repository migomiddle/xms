using System;
using Xms.Business.DuplicateValidator.Domain;

namespace Xms.Business.DuplicateValidator
{
    public interface IDuplicateRuleDependency
    {
        bool Create(DuplicateRule entity);

        bool Delete(params Guid[] id);

        bool Update(DuplicateRule entity);
    }
}