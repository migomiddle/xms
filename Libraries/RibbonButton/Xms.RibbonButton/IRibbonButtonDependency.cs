using System;

namespace Xms.RibbonButton
{
    public interface IRibbonButtonDependency
    {
        bool Create(params Domain.RibbonButton[] entities);

        bool Delete(params Guid[] id);

        bool Update(Domain.RibbonButton entity);
    }
}