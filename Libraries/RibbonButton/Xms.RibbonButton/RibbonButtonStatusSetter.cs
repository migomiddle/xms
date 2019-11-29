using System.Collections.Generic;
using Xms.Core.Data;
using Xms.Form.Abstractions;

namespace Xms.RibbonButton
{
    /// <summary>
    /// 按钮状态设置
    /// </summary>
    public class RibbonButtonStatusSetter : IRibbonButtonStatusSetter
    {
        public RibbonButtonStatusSetter()
        {
        }

        public void Set(List<Domain.RibbonButton> buttons, FormState? formState = null, Entity record = null)
        {
            foreach (var item in buttons)
            {
                item.SetButtonStatus(formState, record);
            }
        }
    }
}