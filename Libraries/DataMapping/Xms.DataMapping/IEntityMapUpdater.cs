using System;
using System.Collections.Generic;
using Xms.DataMapping.Domain;

namespace Xms.DataMapping
{
    public interface IEntityMapUpdater
    {
        bool Update(EntityMap entity);

        bool UpdateState(IEnumerable<Guid> ids, bool isEnabled);
    }
}