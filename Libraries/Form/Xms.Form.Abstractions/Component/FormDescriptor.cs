using System;
using System.Collections.Generic;

namespace Xms.Form.Abstractions.Component
{
    public sealed class FormDescriptor
    {
        public const FormType DefaultFormType = FormType.Main;

        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsShowNav { get; set; }

        public SectionDescriptor Header { get; set; }
        public SectionDescriptor Footer { get; set; }

        public List<NavGroupDescriptor> NavGroups { get; set; }
        public List<PanelDescriptor> Panels { get; set; }
        //public List<SectionDescriptor> Sections { get; set; }

        public List<Event> Events { get; set; }

        public List<string> ClientResources { get; set; }
        public List<Guid> FormRules { get; set; }
        public string CustomCss { get; set; }
    }
}