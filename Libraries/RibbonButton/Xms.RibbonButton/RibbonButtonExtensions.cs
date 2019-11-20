using Xms.Form.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.RibbonButton.Abstractions;
using Xms.Sdk.Extensions;

namespace Xms.RibbonButton
{
    public static class RibbonButtonExtensions
    {
        /// <summary>
        /// 设置按钮可用/可见状态
        /// </summary>
        /// <param name="button"></param>
        /// <param name="appContext"></param>
        /// <param name="formState"></param>
        /// <param name="record"></param>
        /// <param name="entityPermissions"></param>
        public static void SetButtonStatus(this Domain.RibbonButton button, FormState? formState = null, Core.Data.Entity record = null)
        {
            bool enabled = true;
            bool visibled = true;
            if (button.CommandRules.IsNotEmpty())
            {
                var rules = new CommandDefinition();
                rules = rules.DeserializeFromJson(button.CommandRules);
                if (formState.HasValue && rules.FormStateRules != null && rules.FormStateRules.States.NotEmpty())
                {
                    enabled = !rules.FormStateRules.Enabled;
                    visibled = !rules.FormStateRules.Visibled;
                    foreach (var item in rules.FormStateRules.States)
                    {
                        if (item == formState.Value)
                        {
                            //if (enabled)
                            //{
                            enabled = rules.FormStateRules.Enabled;
                            //}
                            //if (visibled)
                            //{
                            visibled = rules.FormStateRules.Visibled;
                            //}
                            if (!visibled)
                            {
                                break;
                            }
                        }
                    }
                }
                if (visibled && record != null && rules.ValueRules != null && rules.ValueRules.Values.NotEmpty())
                {
                    enabled = !rules.ValueRules.Enabled;
                    visibled = !rules.ValueRules.Visibled;
                    foreach (var item in rules.ValueRules.Values)
                    {
                        //if null value
                        if ((item.Value.IsCaseInsensitiveEqual("null") && record[item.Field] == null)
                            || (item.Value.IsCaseInsensitiveEqual(record.GetStringValue(item.Field))))
                        {
                            //if (enabled)
                            //{
                            enabled = rules.ValueRules.Enabled;
                            //}
                            //if (visibled)
                            //{
                            visibled = rules.ValueRules.Visibled;
                            //}
                            if (!visibled)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            button.IsEnabled = enabled;
            button.IsVisibled = visibled;
        }
    }
}