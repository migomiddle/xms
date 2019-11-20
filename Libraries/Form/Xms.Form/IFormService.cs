using System.Collections.Generic;
using Xms.Form.Abstractions.Component;

namespace Xms.Form
{
    public interface IFormService
    {
        List<Schema.Domain.Attribute> AttributeMetaDatas { get; }
        FormDescriptor Form { get; set; }

        void DeleteOriginalLabels(Domain.SystemForm original);

        IFormService Init(Domain.SystemForm formEntity);

        void UpdateLocalizedLabel(Domain.SystemForm original);
    }
}