using System.Collections.Generic;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core.Data;
using Xms.Form.Abstractions;
using Xms.Identity;
using Xms.Schema.Entity;
using Xms.Security.Domain;

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