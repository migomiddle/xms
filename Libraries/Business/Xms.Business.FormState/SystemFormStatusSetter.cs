using System;
using System.Collections.Generic;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;

namespace Xms.Business.FormStateRule
{
    /// <summary>
    /// 表单状态设置
    /// </summary>
    public class SystemFormStatusSetter : ISystemFormStatusSetter
    {
        private readonly ISystemFormStateRuleService _systemFormStateRuleService;

        public SystemFormStatusSetter(ISystemFormStateRuleService systemFormStateRuleService)
        {
            _systemFormStateRuleService = systemFormStateRuleService;
        }

        public bool IsDisabled(List<Guid> rulesId, Entity data)
        {
            var result = false;
            var stateRules = _systemFormStateRuleService.Query(n => n.Where(f => f.SystemFormStateRuleId.In(rulesId)));
            if (stateRules.NotEmpty())
            {
                foreach (var item in stateRules)
                {
                    var flag = item.IsDisabled(data);
                    if (flag)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
    }
}