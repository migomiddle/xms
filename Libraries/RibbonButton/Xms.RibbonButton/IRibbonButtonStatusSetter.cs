using System.Collections.Generic;
using Xms.Core.Data;
using Xms.Form.Abstractions;

namespace Xms.RibbonButton
{
    public interface IRibbonButtonStatusSetter
    {
        void Set(List<Domain.RibbonButton> buttons, FormState? formState = null, Entity record = null);
    }
}