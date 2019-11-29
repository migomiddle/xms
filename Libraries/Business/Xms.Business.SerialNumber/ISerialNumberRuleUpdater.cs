using System;
using System.Collections.Generic;

namespace Xms.Business.SerialNumber
{
    public interface ISerialNumberRuleUpdater
    {
        bool Update(Domain.SerialNumberRule entity);

        bool UpdateState(IEnumerable<Guid> ids, bool isEnabled);
    }
}