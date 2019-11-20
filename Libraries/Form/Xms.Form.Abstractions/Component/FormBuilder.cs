using Xms.Infrastructure.Utility;

namespace Xms.Form.Abstractions.Component
{
    /// <summary>
    /// 表单对象生成器
    /// </summary>
    public class FormBuilder
    {
        public FormBuilder(string formConfig)
        {
            Form = Form.DeserializeFromJson(formConfig);
        }

        public FormDescriptor Form { get; set; }
    }
}