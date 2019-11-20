using System.Collections.Generic;
using Xms.Schema.Abstractions;

namespace Xms.RibbonButton
{
    public interface IDefaultButtonProvider
    {
        List<Domain.RibbonButton> Get(EntityMaskEnum entityMask);
    }
}