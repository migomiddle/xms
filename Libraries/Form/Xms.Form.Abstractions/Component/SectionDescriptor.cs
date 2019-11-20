using System;
using System.Collections.Generic;

namespace Xms.Form.Abstractions.Component
{
    public sealed class SectionDescriptor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Label { get; set; }
        public bool IsShowLabel { get; set; } = true;

        public bool IsVisible { get; set; } = true;
        public int Columns { get; set; }

        public List<RowDescriptor> Rows { get; set; }

        public CellLabelSettings CellLabelSettings { get; set; }
    }
}