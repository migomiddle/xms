using System;
using System.Collections.Generic;
using Xms.Business.Filter.Domain;

namespace Xms.Business.Filter
{
    public interface IFilterRuleUpdater
    {
        bool Update(FilterRule entity);

        bool UpdateState(IEnumerable<Guid> ids, bool isEnabled);
    }
}