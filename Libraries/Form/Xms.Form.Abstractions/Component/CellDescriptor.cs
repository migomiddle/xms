using System;

namespace Xms.Form.Abstractions.Component
{
    public sealed class CellDescriptor
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public bool IsShowLabel { get; set; } = true;

        public bool IsVisible { get; set; } = true;

        public ControlDescriptor Control { get; set; }

        public int ColSpan { get; set; }
        public int RowSpan { get; set; }
        public string CustomCss { get; set; }
    }
}