using System;
using System.Collections.Generic;

namespace Xms.Form.Abstractions.Component
{
    public sealed class PanelDescriptor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Label { get; set; }

        public bool IsExpanded { get; set; } = true;
        public bool IsShowLabel { get; set; } = true;

        public bool IsVisible { get; set; } = true;

        public string DisplayStyle { get; set; }

        public bool Async { get; set; } = false;

        public List<SectionDescriptor> Sections { get; set; }
    }
}