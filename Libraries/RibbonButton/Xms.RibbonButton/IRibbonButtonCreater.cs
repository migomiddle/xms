using System.Collections.Generic;

namespace Xms.RibbonButton
{
    public interface IRibbonButtonCreater
    {
        bool Create(Domain.RibbonButton entity);

        bool CreateDefaultButtons(Schema.Domain.Entity entity);

        bool CreateMany(List<Domain.RibbonButton> entities);
    }
}