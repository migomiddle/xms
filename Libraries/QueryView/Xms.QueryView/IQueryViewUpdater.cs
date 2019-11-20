using System;
using System.Collections.Generic;

namespace Xms.QueryView
{
    public interface IQueryViewUpdater
    {
        bool Update(Domain.QueryView entity);

        bool UpdateAuthorization(bool isAuthorization, params Guid[] id);

        bool UpdateDefault(Guid entityId, Guid queryViewId);

        bool UpdateState(IEnumerable<Guid> ids, bool isEnabled);
    }
}