using System;

namespace Xms.RibbonButton
{
    public interface IRibbonButtonDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}