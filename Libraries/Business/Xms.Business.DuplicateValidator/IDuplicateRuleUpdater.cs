using System;
using System.Collections.Generic;
using Xms.Business.DuplicateValidator.Domain;

namespace Xms.Business.DuplicateValidator
{
    public interface IDuplicateRuleUpdater
    {
        bool Update(DuplicateRule entity);

        bool UpdateState(IEnumerable<Guid> ids, bool isEnabled);
    }
}