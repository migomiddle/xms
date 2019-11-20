using System;

namespace Xms.Form
{
    public interface ISystemFormUpdater
    {
        bool Update(Domain.SystemForm entity, bool updatedConfig);

        bool UpdateAuthorization(bool isAuthorization, params Guid[] id);

        bool UpdateDefault(Guid entityId, Guid systemFormId);

        bool UpdateState(bool isEnabled, params Guid[] id);
    }
}