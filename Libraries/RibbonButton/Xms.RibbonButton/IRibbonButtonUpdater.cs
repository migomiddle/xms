using System;

namespace Xms.RibbonButton
{
    public interface IRibbonButtonUpdater
    {
        bool Update(Domain.RibbonButton entity);

        bool UpdateState(bool isEnabled, params Guid[] id);

        bool UpdateAuthorization(bool isAuthorization, params Guid[] id);
    }
}